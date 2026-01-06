using SaintsField;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Paps.UnityExtensions.Editor;
using EditorObject = UnityEditor.Editor;
using Paps.Optionals;
using UnityEditor.IMGUI.Controls;

namespace Paps.ValueReferences.Editor
{
    [CustomPropertyDrawer(typeof(ValueReference<>))]
    public class ValueReferencePropertyDrawer : PropertyDrawer, IDisposable
    {
        private VisualElement _mainVisualElement;

        private Label _propertyLabel;
        private Button _pingButton;
        private Toggle _sourceSelectToggle;
        private Toggle _customValueToggle;
        private IMGUIContainer _selectSourceButtonContainer;
        private VisualElement _sourceEditorContainer;
        private Toggle _hasCustomValueToggle;
        private VisualElement _customValueEditorContainer;

        private VisualElement _sourceSelectViewContainer;
        private VisualElement _customValueViewContainer;

        private SerializedProperty _interfaceUnityObjectProperty;
        private SerializedProperty _referenceSourceProperty;
        private SerializedProperty _optionalCustomValueProperty;
        private SerializedProperty _hasCustomValueProperty;
        private SerializedProperty _customValueProperty;
        private object _valueReferenceObject;
        private Type _valueReferenceTypeParameter;
        private EditorObject _valueSourceEditor;
        private PropertyField _customValuePropertyField;

        private bool _selectSourceButtonClicked;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var visualTreeAsset = ValueReferencesEditorConfig.Instance.ValueReferencePropertyDrawerVTA;
            _mainVisualElement = visualTreeAsset.CloneTree();

            var propertyLabel = _mainVisualElement.Q<Label>("PropertyNameLabel");

            _sourceSelectToggle = _mainVisualElement.Q<ToolbarToggle>("SourceSelectViewToggle");
            _customValueToggle = _mainVisualElement.Q<ToolbarToggle>("CustomValueViewToggle");

            _sourceSelectViewContainer = _mainVisualElement.Q("SourceSelectViewContainer");
            _customValueViewContainer = _mainVisualElement.Q("CustomValueViewContainer");
            
            _pingButton = _mainVisualElement.Q<Button>("PingButton");
            _selectSourceButtonContainer = _mainVisualElement.Q<IMGUIContainer>("SelectSourceButtonContainer");
            _sourceEditorContainer = _mainVisualElement.Q("SourceEditorContainer");

            _hasCustomValueToggle = _mainVisualElement.Q<Toggle>("HasCustomValueToggle");
            _customValueEditorContainer = _mainVisualElement.Q("CustomValueEditorContainer");

            _referenceSourceProperty = property.FindPropertyRelative("_referenceSource");
            _interfaceUnityObjectProperty = _referenceSourceProperty.FindPropertyRelativeBakingField(nameof(SaintsInterface<object>.V));

            _optionalCustomValueProperty = property.FindPropertyRelative("_hardcodedValue");
            _hasCustomValueProperty = _optionalCustomValueProperty.FindPropertyRelative("_considerItHasValue");
            _customValueProperty = _optionalCustomValueProperty.FindPropertyRelative("_value");

            _valueReferenceObject = property.GetTargetObject();
            _valueReferenceTypeParameter = _valueReferenceObject.GetType().GetGenericArguments().First();

            propertyLabel.text = property.displayName + $" <color=red>({_valueReferenceTypeParameter.Name})</color>";

            _sourceSelectToggle.RegisterCallback<ChangeEvent<bool>>(ev =>
            {
                if(ev.newValue)
                    SwitchToSourceSelectView();
                else
                    SwitchToCustomValueView();
            });

            _customValueToggle.RegisterCallback<ChangeEvent<bool>>(ev =>
            {
                if(ev.newValue)
                    SwitchToCustomValueView();
                else
                    SwitchToSourceSelectView();
            });

            ShowInitialView();

            _pingButton.clicked += () =>
            {
                EditorGUIUtility.PingObject(_interfaceUnityObjectProperty.objectReferenceValue);
            };

            _selectSourceButtonContainer.onGUIHandler += SourceSelectButtonOnGUI;

            UpdateValueSourceEditor();

            _hasCustomValueToggle.BindProperty(_hasCustomValueProperty);

            _customValuePropertyField = new PropertyField(_customValueProperty);
            _customValueEditorContainer.Add(_customValuePropertyField);

            return _mainVisualElement;
        }

        private void SourceSelectButtonOnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(GetSelectSourceButtonStringState()))
            {
                _selectSourceButtonClicked = true;
            }
            
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            if(_selectSourceButtonClicked && Event.current.type == EventType.Repaint)
            {
                ShowDropdown();
                _selectSourceButtonClicked = false;
            }
        }

        private void ShowInitialView()
        {
            if(_interfaceUnityObjectProperty.objectReferenceValue == null && _hasCustomValueProperty.boolValue)
            {
                SwitchToCustomValueView();
                return;
            }

            SwitchToSourceSelectView();
        }

        private void ShowDropdown()
        {
            var rect = GUILayoutUtility.GetLastRect();

            var advancedDropdown = GetDropdrownForType(_valueReferenceTypeParameter);
            advancedDropdown.OnSelected += OnSourceSelected;
            advancedDropdown.Show(rect);
        }

        private ValueReferencesAdvancedDropdown GetDropdrownForType(Type valueReferenceTypeParameter)
        {
            var staticType = typeof(ValueReferencesUIUtils);

            var method = staticType.GetMethod(nameof(ValueReferencesUIUtils.CreateAdvancedDropdownForType), BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(valueReferenceTypeParameter);

            return (ValueReferencesAdvancedDropdown)method.Invoke(null, null);
        }

        private void OnSourceSelected(ValueReferenceAsset asset)
        {
            _interfaceUnityObjectProperty.objectReferenceValue = asset;

            _interfaceUnityObjectProperty.serializedObject.ApplyModifiedProperties();
        }

        private string GetSelectSourceButtonStringState()
        {
            var valueReferenceAsset = _interfaceUnityObjectProperty.objectReferenceValue as ValueReferenceAsset;

            if(valueReferenceAsset == null)
            {
                return "No source selected";
            }
            else
            {
                return valueReferenceAsset.GetPath();
            }
        }

        private void UpdateValueSourceEditor()
        {
            EditorObject.DestroyImmediate(_valueSourceEditor);
            _valueSourceEditor = EditorObject.CreateEditor(_interfaceUnityObjectProperty.objectReferenceValue);

            _sourceEditorContainer.Clear();
            
            if(_valueSourceEditor != null)
            {
                var editorElement = _valueSourceEditor.CreateInspectorGUI();
                _sourceEditorContainer.Add(editorElement);
            }
                
        }

        private void SwitchToSourceSelectView()
        {
            _customValueToggle.SetValueWithoutNotify(false);
            _customValueViewContainer.style.display = DisplayStyle.None;
            _customValueEditorContainer.style.display = DisplayStyle.None;
                
            _sourceSelectToggle.SetValueWithoutNotify(true);
            _sourceSelectViewContainer.style.display = DisplayStyle.Flex;
            _sourceEditorContainer.style.display = DisplayStyle.Flex;
        }

        private void SwitchToCustomValueView()
        {
            _sourceSelectToggle.SetValueWithoutNotify(false);
            _sourceSelectViewContainer.style.display = DisplayStyle.None;
            _sourceEditorContainer.style.display = DisplayStyle.None;

            _customValueToggle.SetValueWithoutNotify(true);
            _customValueViewContainer.style.display = DisplayStyle.Flex;
            _customValueEditorContainer.style.display = DisplayStyle.Flex;
        }

        public void Dispose()
        {
            EditorObject.DestroyImmediate(_valueSourceEditor);
        }
    }
}
