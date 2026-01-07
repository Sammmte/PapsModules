using UnityEditor;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using Paps.UnityExtensions.Editor;
using System;
using System.IO;

namespace Paps.ValueReferences.Editor
{
    [CustomEditor(typeof(ValueReferenceGroupAsset))]
    public class ValueReferenceGroupAssetEditor : EditorObject
    {
        [SerializeField] private VisualTreeAsset _editorUI;

        [SerializeField] private VisualTreeAsset _valueReferenceElementUI;

        private VisualElement _mainVisualElement;
        private Label _groupNameLabel;
        private PropertyField _pathField;
        private Button _addButton;
        private Button _renameButton;
        private TextField _renameTextField;
        private VisualElement _itemsContainer;

        private List<ValueReferenceElement> _elements;
        private ValueReferenceGroupAsset _groupAsset;

        private GenericMenu _addValueReferenceAssetMenu;

        private SerializedProperty _valueReferencesArrayProperty;
        private SerializedProperty _pathProperty;

        public override VisualElement CreateInspectorGUI()
        {
            _groupAsset = target as ValueReferenceGroupAsset;
            _elements = new List<ValueReferenceElement>();

            _mainVisualElement = _editorUI.CloneTree();

            _groupNameLabel = _mainVisualElement.Q<Label>("GroupNameLabel");
            _pathField = _mainVisualElement.Q<PropertyField>("PathField");
            _addButton = _mainVisualElement.Q<Button>("AddButton");
            _renameButton = _mainVisualElement.Q<Button>("RenameButton");
            _renameTextField = _mainVisualElement.Q<TextField>("RenameTextField");
            _itemsContainer = _mainVisualElement.Q("ItemsContainer");

            _valueReferencesArrayProperty = serializedObject.FindProperty(nameof(ValueReferenceGroupAsset.ValueReferenceAssets));

            RefreshName(); 

            _pathProperty = serializedObject.FindPropertyBakingField(nameof(ValueReferenceGroupAsset.Path));

            _pathField.BindProperty(_pathProperty);
            _pathField.TrackPropertyValue(_pathProperty, property =>
            {
                ValueReferencesEditorManager.RefreshPaths();
            });
            _addButton.clicked += OnAddButtonClicked;
            _renameButton.clicked += OnRenameButtonClicked;

            _renameTextField.RegisterCallback<ChangeEvent<string>>(ev =>
            {
                Rename(ev.newValue);
            });

            _renameTextField.RegisterCallback<FocusOutEvent>(ev =>
            {
                HideRenameView();
            });

            _addValueReferenceAssetMenu = new GenericMenu();
            
            var createAssetMenuAttributesPerType = ValueReferencesEditorManager.GetCreateAssetMenuPerType();

            foreach(var tuple in createAssetMenuAttributesPerType)
            {
                _addValueReferenceAssetMenu.AddItem(new GUIContent(tuple.Attribute.menuName), 
                    false, OnNewValueReferenceAssetSelected, tuple.Type);
            }

            if(IsOrphanGroup())
            {
                ShowAsOrphanGroup();
            }

            RefreshItems();

            return _mainVisualElement;
        }

        private bool IsOrphanGroup() => _pathProperty.stringValue == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME;

        private ValueReferenceElement ToUIElement(ValueReferenceAsset asset)
        {
            var elementParent = _valueReferenceElementUI.CloneTree();
            var element = elementParent.Q<ValueReferenceElement>();

            element.Initialize(asset, new ValueReferenceElement.Options()
            { 
                HideDelete = IsOrphanGroup() 
            });

            element.OnDeleteRequested += OnItemDeleteRequested;

            return element;
        }

        private void OnItemDeleteRequested(ValueReferenceElement element)
        {
            if(EditorUtility.DisplayDialog(
                "Remove asset from group", 
                $"HEADS UP! This will NOT delete the item, only remove it from {_groupAsset.name} group.\nDo you wish to continue?",
                "Accept", "Cancel"))
            {
                RemoveItem(element);
                RefreshItems();
                ValueReferencesEditorManager.RefreshAll();
            }
        }

        private void RemoveItem(ValueReferenceElement element)
        {
            serializedObject.Update();

            var index = IndexOf(element.ValueReferenceAsset);

            _valueReferencesArrayProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }

        private int IndexOf(ValueReferenceAsset asset)
        {
            for(int i = 0; i < _groupAsset.ValueReferenceAssets.Length; i++)
            {
                if(_groupAsset.ValueReferenceAssets[i] == asset)
                    return i;
            }

            return -1;
        }

        private void OnAddButtonClicked()
        {
            _addValueReferenceAssetMenu.ShowAsContext();
        }

        private void OnNewValueReferenceAssetSelected(object typeData)
        {
            var type = typeData as Type;

            var newAsset = ScriptableObject.CreateInstance(type);

            serializedObject.Update();

            var newIndex = _valueReferencesArrayProperty.arraySize;

            _valueReferencesArrayProperty.InsertArrayElementAtIndex(newIndex);

            var newProperty = _valueReferencesArrayProperty.GetArrayElementAtIndex(newIndex);

            var thisGroupPath = AssetDatabase.GetAssetPath(_groupAsset);

            var folderPath = Path.GetDirectoryName(thisGroupPath);

            AssetDatabase.CreateAsset(newAsset, Path.Combine(folderPath, $"New{type.Name}ValueReference.asset"));

            newProperty.objectReferenceValue = newAsset;

            serializedObject.ApplyModifiedProperties();

            RefreshItems();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var lastElement = _elements.Last();

            lastElement.ShowRenameView();
        }

        private void RefreshItems()
        {
            foreach(var element in _elements)
            {
                element.Dispose();
            }

            _elements.Clear();
            serializedObject.Update();
            _elements.AddRange(_groupAsset.ValueReferenceAssets.Select(ToUIElement));

            _itemsContainer.Clear();
            foreach(var element in _elements)
            {
                _itemsContainer.Add(element);
            }
        }

        private void OnRenameButtonClicked()
        {
            ShowRenameView();
        }

        private void ShowRenameView()
        {
            _renameTextField.style.display = DisplayStyle.Flex;
            _renameTextField.SetValueWithoutNotify(_groupAsset.name);
            _renameTextField.Focus();

            _groupNameLabel.style.display = DisplayStyle.None;
        }

        private void HideRenameView()
        {
            _renameTextField.style.display = DisplayStyle.None;

            _groupNameLabel.style.display = DisplayStyle.Flex;
        }

        private void Rename(string newName)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_groupAsset), newName);

            RefreshName();
        }

        private void RefreshName()
        {
            _groupNameLabel.text = _groupAsset.name;
        }

        private void ShowAsOrphanGroup()
        {
            _renameButton.style.display = DisplayStyle.None;
            _renameButton.enabledSelf = false;

            _addButton.style.display = DisplayStyle.None;
            _addButton.enabledSelf = false;

            _pathField.style.display = DisplayStyle.None;
            _pathField.enabledSelf = false;
        }
    }
}
