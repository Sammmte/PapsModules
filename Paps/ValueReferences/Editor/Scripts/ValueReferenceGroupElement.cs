using Paps.UnityExtensions.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceGroupElement : VisualElement
    {
        private Foldout _mainFoldout;
        private VisualElement _valueReferencesContainer;
        private VisualElement _subGroupsContainer;
        private Button _renameButton;
        private TextField _renameTextField;
        private Button _deleteButton;
        private DropdownField _valueReferencesTypeSelectorDropdown;
        private Button _addValueReferenceButton;
        private Button _addSubGroupButton;

        private ValueReferenceGroupAsset _groupAsset;
        
        private SerializedProperty _groupProperty;
        private SerializedProperty _groupPathProperty;
        private SerializedProperty _valueReferencesProperty;
        private SerializedProperty _subGroupsProperty;

        private VisualTreeAsset _valueReferenceElementVTA;
        private VisualTreeAsset _valueReferenceGroupElementVTA;

        private Dictionary<string, Type> _valueReferenceTypes;
        private bool _isRoot;
        
        private List<ValueReferenceElement> _valueReferenceElements;
        private List<ValueReferenceGroupElement> _subGroupElements;
        
        public event Action<ValueReferenceGroupElement> OnDeleteRequested;
        
        public void Initialize(ValueReferenceGroupAsset groupAsset, SerializedProperty groupProperty, VisualTreeAsset valueReferenceElementVTA,
            VisualTreeAsset valueReferenceGroupElementVTA, Dictionary<string, Type> valueReferenceTypes, bool isRoot)
        {
            _groupAsset = groupAsset;
            
            _groupProperty = groupProperty;
            _groupPathProperty = groupProperty.FindPropertyRelativeBakingField("GroupPath");
            _valueReferencesProperty = groupProperty.FindPropertyRelativeBakingField("ValueReferences");
            _subGroupsProperty = groupProperty.FindPropertyRelativeBakingField("SubGroups");

            _valueReferenceElementVTA = valueReferenceElementVTA;
            _valueReferenceGroupElementVTA = valueReferenceGroupElementVTA;

            _valueReferenceTypes = valueReferenceTypes;
            _isRoot = isRoot;
            
            _valueReferenceElements = new List<ValueReferenceElement>();
            _subGroupElements = new List<ValueReferenceGroupElement>();
            
            _mainFoldout = this.Q<Foldout>("MainFoldout");
            _valueReferencesContainer = this.Q("ValueReferencesFoldout");
            _subGroupsContainer = this.Q("SubGroupsFoldout");
            _renameButton = this.Q<Button>("RenameButton");
            _renameTextField = this.Q<TextField>("RenameTextField");
            _deleteButton = this.Q<Button>("DeleteButton");
            _valueReferencesTypeSelectorDropdown = this.Q<DropdownField>("ValueReferenceTypeSelector");
            _addValueReferenceButton = this.Q<Button>("AddValueReferenceButton");
            _addSubGroupButton = this.Q<Button>("AddSubGroupButton");

            InitializeSelfElements();
            InitializeValueReferences();
            InitializeSubGroups();
        }

        private void InitializeSelfElements()
        {
            UpdatePath();
            _valueReferencesTypeSelectorDropdown.choices = _valueReferenceTypes.Keys.ToList();
            _valueReferencesTypeSelectorDropdown.SetValueWithoutNotify(_valueReferencesTypeSelectorDropdown.choices[0]);
            
            _renameButton.clicked += ShowRenameField;
            _renameTextField.RegisterValueChangedCallback(OnRenamed);

            _deleteButton.clicked += OnDelete;

            _addValueReferenceButton.clicked += AddValueReferenceElement;
            _addSubGroupButton.clicked += AddSubGroupElement;

            if (_isRoot)
            {
                _deleteButton.style.display = DisplayStyle.None;
            }
        }

        private void OnRenamed(ChangeEvent<string> ev)
        {
            if (!string.IsNullOrEmpty(ev.newValue))
            {
                UpdatePath(ev.newValue);
            }

            _renameTextField.SetValueWithoutNotify(string.Empty);
            _renameTextField.style.display = DisplayStyle.None;
        }

        private void AddValueReferenceElement()
        {
            _valueReferencesProperty.arraySize += 1;
            var newIndex = _valueReferencesProperty.arraySize - 1;

            var newValueReferenceProperty = _valueReferencesProperty.GetArrayElementAtIndex(newIndex);
            var newAsset =
                ScriptableObject.CreateInstance(_valueReferenceTypes[_valueReferencesTypeSelectorDropdown.value]);
            newAsset.name = _valueReferenceTypes[_valueReferencesTypeSelectorDropdown.value].Name;
            
            AssetDatabase.AddObjectToAsset(newAsset, AssetDatabase.GetAssetPath(_groupAsset));
            
            newValueReferenceProperty.objectReferenceValue = newAsset;
            
            var newElementParent = _valueReferenceElementVTA.CloneTree();

            var newElement = newElementParent.Q<ValueReferenceElement>();
                
            newElement.Initialize(newValueReferenceProperty);
                
            _valueReferenceElements.Add(newElement);
            _valueReferencesContainer.Add(newElement);

            newElement.OnDeleteRequested += OnValueReferenceRequestedDelete;

            newValueReferenceProperty.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private void AddSubGroupElement()
        {
            _subGroupsProperty.arraySize += 1;
            var newIndex = _subGroupsProperty.arraySize - 1;

            SetupNewSubGroupElementAtIndex(newIndex);
        }

        private void SetupNewSubGroupElementAtIndex(int index)
        {
            var newSubGroupProperty = SetupSubGroupElementAtIndex(index);

            newSubGroupProperty.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private SerializedProperty SetupSubGroupElementAtIndex(int index)
        {
            var newSubGroupProperty = _subGroupsProperty.GetArrayElementAtIndex(index);

            var newElementParent = _valueReferenceGroupElementVTA.CloneTree();

            var newElement = newElementParent.Q<ValueReferenceGroupElement>();
            
            newElement.Initialize(_groupAsset, newSubGroupProperty, _valueReferenceElementVTA, 
                _valueReferenceGroupElementVTA, _valueReferenceTypes, false);
            
            _subGroupElements.Add(newElement);
            _subGroupsContainer.Add(newElement);

            newElement.OnDeleteRequested += OnValueReferenceGroupRequestedDelete;

            return newSubGroupProperty;
        }

        private void UpdatePath(string path)
        {
            _groupPathProperty.stringValue = path;
            _groupPathProperty.serializedObject.ApplyModifiedProperties();
            UpdatePath();
        }

        private void UpdatePath()
        {
            if (string.IsNullOrEmpty(_groupPathProperty.stringValue))
            {
                _groupPathProperty.stringValue = _groupAsset.name;
                _groupPathProperty.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
            
            _mainFoldout.text = _groupPathProperty.stringValue;
        }

        private void ShowRenameField()
        {
            _renameTextField.SetValueWithoutNotify(_groupPathProperty.stringValue);
            _renameTextField.style.display = DisplayStyle.Flex;
        }

        private void OnDelete()
        {
            OnDeleteRequested?.Invoke(this);
        }

        private void InitializeValueReferences()
        {
            for (int i = 0; i < _valueReferencesProperty.arraySize; i++)
            {
                var newElementParent = _valueReferenceElementVTA.CloneTree();

                var newElement = newElementParent.Q<ValueReferenceElement>();
                
                newElement.Initialize(_valueReferencesProperty.GetArrayElementAtIndex(i));
                
                _valueReferenceElements.Add(newElement);
                _valueReferencesContainer.Add(newElement);

                newElement.OnDeleteRequested += OnValueReferenceRequestedDelete;
            }
        }

        private void OnValueReferenceRequestedDelete(ValueReferenceElement valueReferenceElement)
        {
            var index = _valueReferenceElements.IndexOf(valueReferenceElement);
            
            _valueReferenceElements.Remove(valueReferenceElement);
            _valueReferencesContainer.Remove(valueReferenceElement);

            var elementProperty = _valueReferencesProperty.GetArrayElementAtIndex(index);
            Object.DestroyImmediate(elementProperty.objectReferenceValue, true);
            _valueReferencesProperty.DeleteArrayElementAtIndex(index);

            _valueReferencesProperty.serializedObject.ApplyModifiedProperties();
            
            AssetDatabase.SaveAssets();
        }

        private void InitializeSubGroups()
        {
            for (int i = 0; i < _subGroupsProperty.arraySize; i++)
            {
                SetupSubGroupElementAtIndex(i);
            }
        }

        private void OnValueReferenceGroupRequestedDelete(ValueReferenceGroupElement valueReferenceGroupElement)
        {
            var index = _subGroupElements.IndexOf(valueReferenceGroupElement);
            
            _subGroupElements.Remove(valueReferenceGroupElement);
            _subGroupsContainer.Remove(valueReferenceGroupElement);

            for (int i = 0; i < valueReferenceGroupElement._valueReferencesProperty.arraySize; i++)
            {
                var elementProperty = valueReferenceGroupElement._valueReferencesProperty.GetArrayElementAtIndex(index);
                Object.DestroyImmediate(elementProperty.objectReferenceValue, true);
            }
            
            _subGroupsProperty.DeleteArrayElementAtIndex(index);

            _subGroupsProperty.serializedObject.ApplyModifiedProperties();
            
            AssetDatabase.SaveAssets();
        }
    }
}