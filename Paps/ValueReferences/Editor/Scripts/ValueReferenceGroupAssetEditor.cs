using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Paps.ValueReferences.Editor
{
    [CustomEditor(typeof(ValueReferenceGroupAsset))]
    public class ValueReferenceGroupAssetEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _editorVTA;
        [SerializeField] private VisualTreeAsset _valueReferenceElementVTA;

        private Dictionary<string, Type> _valueReferenceTypes;

        private DropdownField _typeSelectorDropdown;
        private ScrollView _elementsContainer;
        private SerializedProperty _valueReferencesProperty;

        private List<ValueReferenceElement> _valueReferenceElements = new List<ValueReferenceElement>();
        
        public override VisualElement CreateInspectorGUI()
        {
            var editorElement = _editorVTA.CloneTree();

            var groupPathField = editorElement.Q<PropertyField>("GroupPathField");
            _typeSelectorDropdown = editorElement.Q<DropdownField>("ReferenceTypeSelectorDropdown");
            var createButton = editorElement.Q<Button>("CreateButton");
            _elementsContainer = editorElement.Q<ScrollView>();
            
            _valueReferenceTypes = GetValueReferenceTypes();
            _valueReferencesProperty = serializedObject.FindProperty("_valueReferences");
            
            var groupPathProperty = serializedObject.FindProperty("<GroupPath>k__BackingField");
            groupPathField.bindingPath = groupPathProperty.propertyPath;

            _typeSelectorDropdown.choices = _valueReferenceTypes.Keys.ToList();
            _typeSelectorDropdown.SetValueWithoutNotify(_typeSelectorDropdown.choices[0]);

            createButton.clicked += CreateValueReferenceOfSelectedType;
            
            RefreshItems();

            return editorElement;
        }

        private void CreateValueReferenceOfSelectedType()
        {
            var type = _valueReferenceTypes[_typeSelectorDropdown.value];

            var newReferenceAsset = CreateInstance(type) as ValueReferenceAsset;
            newReferenceAsset.name = _typeSelectorDropdown.value + "ValueReference";
            
            AssetDatabase.AddObjectToAsset(newReferenceAsset, target);

            _valueReferencesProperty.arraySize += 1;

            var newAssetProperty = _valueReferencesProperty.GetArrayElementAtIndex(_valueReferencesProperty.arraySize - 1);

            newAssetProperty.objectReferenceValue = newReferenceAsset;

            serializedObject.ApplyModifiedProperties();
            
            AssetDatabase.SaveAssets();
            
            RefreshItems();
        }

        private void RefreshItems()
        {
            _valueReferenceElements.Clear();
            _elementsContainer.Clear();

            var arraySize = _valueReferencesProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                var valueReferenceAsset =
                    _valueReferencesProperty.GetArrayElementAtIndex(i).objectReferenceValue as ValueReferenceAsset;
                var valueReferenceElementParent = _valueReferenceElementVTA.CloneTree();
                var valueReferenceElement = valueReferenceElementParent.Q<ValueReferenceElement>();
                
                valueReferenceElement.Initialize(valueReferenceAsset, i);
                valueReferenceElement.OnDeleteRequested += DeleteValueReference;
                
                _valueReferenceElements.Add(valueReferenceElement);
                _elementsContainer.Add(valueReferenceElement);
            }
        }

        private void DeleteValueReference(ValueReferenceElement valueReferenceElement)
        {
            _valueReferencesProperty.DeleteArrayElementAtIndex(valueReferenceElement.Index);
            DestroyImmediate(valueReferenceElement.ValueReferenceAsset, true);

            serializedObject.ApplyModifiedProperties();
            
            AssetDatabase.SaveAssets();
            
            RefreshItems();
        }

        private Dictionary<string, Type> GetValueReferenceTypes()
        {
            return TypeCache.GetTypesDerivedFrom(typeof(ValueReferenceAsset<>))
                .Where(type => type != typeof(ValueReferenceAsset<>))
                .ToDictionary(type => type.BaseType.GetGenericArguments().First().Name, type => type);
        }
    }
}
