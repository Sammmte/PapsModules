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
        private Button _selectSourceButton;
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
        private IMGUIContainer _advancedDropdownStyleInitializationContainer;

        private bool _loadedAdvancedDropdownStyles;

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
            _selectSourceButton = _mainVisualElement.Q<Button>("SelectSourceButton");
            _sourceEditorContainer = _mainVisualElement.Q("SourceEditorContainer");

            _hasCustomValueToggle = _mainVisualElement.Q<Toggle>("HasCustomValueToggle");
            _customValueEditorContainer = _mainVisualElement.Q("CustomValueEditorContainer");

            _advancedDropdownStyleInitializationContainer = new IMGUIContainer(ShowDropdownOnGUI);
            _mainVisualElement.Add(_advancedDropdownStyleInitializationContainer);

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

            SwitchToSourceSelectView();

            _pingButton.clicked += () =>
            {
                EditorGUIUtility.PingObject(_interfaceUnityObjectProperty.objectReferenceValue);
            };

            UpdateSelectSourceButtonState();

            _selectSourceButton.clicked += () =>
            {
                _mainVisualElement.Remove(_advancedDropdownStyleInitializationContainer);
                ShowDropdown();
            };

            UpdateValueSourceEditor();

            _hasCustomValueToggle.BindProperty(_hasCustomValueProperty);

            _customValuePropertyField = new PropertyField(_customValueProperty);
            _customValueEditorContainer.Add(_customValuePropertyField);

            return _mainVisualElement;
        }

        private void ShowDropdownOnGUI()
        {
            if(_loadedAdvancedDropdownStyles)
                return;

            var advancedDropdownGUIType = typeof(AdvancedDropdown).Assembly.GetType("UnityEditor.IMGUI.Controls.AdvancedDropdownGUI");
            var loadStylesMethod = advancedDropdownGUIType.GetMethod("LoadStyles", BindingFlags.NonPublic | BindingFlags.Static);

            loadStylesMethod.Invoke(null, null);

            _loadedAdvancedDropdownStyles = true;
        }

        private void ShowDropdown()
        {
            var advancedDropdown = GetDropdrownForType(_valueReferenceTypeParameter);
            advancedDropdown.OnSelected += OnSourceSelected;
            advancedDropdown.Show(_selectSourceButton.worldBound);
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

            UpdateSelectSourceButtonState();
        }

        private void UpdateSelectSourceButtonState()
        {
            var valueReferenceAsset = _interfaceUnityObjectProperty.objectReferenceValue as ValueReferenceAsset;

            if(valueReferenceAsset == null)
            {
                _selectSourceButton.text = "No source selected";
            }
            else
            {
                _selectSourceButton.text = valueReferenceAsset.GetPath();
            }
        }

        private void UpdateValueSourceEditor()
        {
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
