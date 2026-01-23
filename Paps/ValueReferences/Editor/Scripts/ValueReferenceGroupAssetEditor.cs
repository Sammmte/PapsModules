using UnityEditor;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Paps.UnityExtensions.Editor;

namespace Paps.ValueReferences.Editor
{
    [CustomEditor(typeof(ValueReferenceGroupAsset))]
    public class ValueReferenceGroupAssetEditor : EditorObject
    {
        [SerializeField] private VisualTreeAsset _editorUI;
        [SerializeField] private VisualTreeAsset _valueReferenceNameElementUI;
        [SerializeField] private VisualTreeAsset _valueReferenceTypeElementUI;
        [SerializeField] private VisualTreeAsset _valueReferenceValueElementUI;
        [SerializeField] private VisualTreeAsset _valueReferenceExtraControlsElementUI;

        private VisualElement _mainVisualElement;
        private Label _groupNameLabel;
        private PropertyField _pathField;
        private ValueReferenceGroupAddButton _addButton;
        private Button _renameButton;
        private Button _pingButton;
        private TextField _renameTextField;
        private MultiColumnListView _itemsContainer;
        private ValueReferenceGroupAsset _groupAsset;
        private List<ValueReferenceAsset> _itemsSource;
        private SerializedProperty _pathProperty;

        public override VisualElement CreateInspectorGUI()
        {
            _groupAsset = target as ValueReferenceGroupAsset;
            _itemsSource = new List<ValueReferenceAsset>();

            _mainVisualElement = _editorUI.CloneTree();

            _groupNameLabel = _mainVisualElement.Q<Label>("GroupNameLabel");
            _pathField = _mainVisualElement.Q<PropertyField>("PathField");
            _addButton = _mainVisualElement.Q<ValueReferenceGroupAddButton>();
            _renameButton = _mainVisualElement.Q<Button>("RenameButton");
            _pingButton = _mainVisualElement.Q<Button>("PingButton");
            _renameTextField = _mainVisualElement.Q<TextField>("RenameTextField");
            _itemsContainer = _mainVisualElement.Q<MultiColumnListView>("ItemsContainer");

            RefreshName();

            _pathProperty = serializedObject.FindPropertyBakingField(nameof(ValueReferenceGroupAsset.Path));

            _pathField.BindProperty(_pathProperty);
            _pathField.TrackPropertyValue(_pathProperty, property =>
            {
                ValueReferencesEditorManager.RefreshPaths();
            });

            if(_groupAsset.ValueReferenceAssets != null)
            {
                _itemsSource.AddRange(_groupAsset.ValueReferenceAssets);
            }
            
            _itemsContainer.itemsSource = _itemsSource;

            _itemsContainer.dragAndDropUpdate += OnDragAndDropUpdate;
            _itemsContainer.handleDrop += OnHandleDrop;

            SetupColumns();

            _addButton.Initialize(_groupAsset, OnNewItemAdded);
            _renameButton.clicked += OnRenameButtonClicked;
            _pingButton.clicked += PingAsset;

            _renameTextField.RegisterCallback<ChangeEvent<string>>(ev =>
            {
                Rename(ev.newValue);
                HideRenameView();
            });

            _renameTextField.RegisterCallback<FocusOutEvent>(ev =>
            {
                HideRenameView();
            });

            if (IsOrphanGroup())
            {
                ShowAsOrphanGroup();
            }

            _itemsContainer.Rebuild();

            return _mainVisualElement;
        }

        private DragVisualMode OnDragAndDropUpdate(HandleDragAndDropArgs dragArgs)
        {
            if(dragArgs.dragAndDropData.paths != null && 
                dragArgs.dragAndDropData.paths.Length > 0 &&
                TryGetValueReferenceAssets(dragArgs.dragAndDropData.paths, out var assets))
            {
                return DragVisualMode.Move;
            }

            return DragVisualMode.Rejected;
        }

        private DragVisualMode OnHandleDrop(HandleDragAndDropArgs dragArgs)
        {
            if(dragArgs.dragAndDropData.paths != null && 
                dragArgs.dragAndDropData.paths.Length > 0 &&
                TryGetValueReferenceAssets(dragArgs.dragAndDropData.paths, out var assets))
            {
                ReceiveAddedAssets(assets);
                return DragVisualMode.Move;
            }

            return DragVisualMode.Rejected;
        }

        private bool TryGetValueReferenceAssets(string[] paths, out ValueReferenceAsset[] assets)
        {
            assets = new ValueReferenceAsset[paths.Length];

            for(int i = 0; i < paths.Length; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<ValueReferenceAsset>(paths[i]);

                if(asset == null)
                    return false;

                assets[i] = asset;
            }

            return true;
        }

        private void SetupColumns()
        {
            var nameColumn = _itemsContainer.columns["Name"];
            var typeColumn = _itemsContainer.columns["Type"];
            var valueColumn = _itemsContainer.columns["Value"];
            var extraControlsColumn = _itemsContainer.columns["ExtraControls"];

            nameColumn.makeCell += CreateNameElement;
            nameColumn.bindCell += BindNameElement;
            nameColumn.unbindCell += UnbindNameElement;

            typeColumn.makeCell += CreateTypeElement;
            typeColumn.bindCell += BindTypeElement;
            typeColumn.unbindCell += UnbindTypeElement;

            valueColumn.makeCell += CreateValueElement;
            valueColumn.bindCell += BindValueElement;
            valueColumn.unbindCell += UnbindValueElement;

            extraControlsColumn.makeCell += CreateExtraControlsElement;
            extraControlsColumn.bindCell += BindExtraControlsElement;
            extraControlsColumn.unbindCell += UnbindExtraControlsElement;
        }

        private void OnNewItemAdded(ValueReferenceAsset asset)
        {
            _itemsSource.Add(asset);

            _itemsContainer.RefreshItems();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ValueReferencesEditorManager.RefreshPaths();

            var lastItem = _itemsContainer.GetRootElementForIndex(_itemsSource.Count - 1);

            var lastNameElement = lastItem.Q<ValueReferenceNameElement>();

            lastNameElement.ShowRenameView();
        }

        private void OnElementRemoved(ValueReferenceAsset asset)
        {
            _itemsSource.Remove(asset);

            _itemsContainer.RefreshItems();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ValueReferencesEditorManager.RefreshPaths();
        }

        private void ReceiveAddedAssets(ValueReferenceAsset[] addedAssets)
        {
            ValueReferencesEditorManager.MoveValueReferencesAssets(addedAssets, _groupAsset);
            Refresh();
        }

        private VisualElement CreateNameElement()
        {
            var parent = _valueReferenceNameElementUI.CloneTree();

            var element = parent.Q<ValueReferenceNameElement>();

            element.Initialize();

            parent.Remove(element);

            return element;
        }

        private void BindNameElement(VisualElement element, int index)
        {
            var nameElement = element as ValueReferenceNameElement;

            var data = _itemsSource[index];

            nameElement.SetData(data);
        }

        private void UnbindNameElement(VisualElement element, int index)
        {
            var nameElement = element as ValueReferenceNameElement;

            nameElement.CleanUp();
        }

        private VisualElement CreateTypeElement()
        {
            var parent = _valueReferenceTypeElementUI.CloneTree();

            var element = parent.Q<ValueReferenceTypeElement>();

            element.Initialize();

            parent.Remove(element);

            return element;
        }

        private void BindTypeElement(VisualElement element, int index)
        {
            var typeElement = element as ValueReferenceTypeElement;

            var data = _itemsSource[index];

            typeElement.SetData(data);
        }

        private void UnbindTypeElement(VisualElement element, int index)
        {
            var typeElement = element as ValueReferenceTypeElement;

            typeElement.CleanUp();
        }

        private VisualElement CreateValueElement()
        {
            var parent = _valueReferenceValueElementUI.CloneTree();

            var element = parent.Q<ValueReferenceValueElement>();

            element.Initialize();

            parent.Remove(element);

            return element;
        }

        private void BindValueElement(VisualElement element, int index)
        {
            var valueElement = element as ValueReferenceValueElement;

            var data = _itemsSource[index];

            valueElement.SetData(data);
        }

        private void UnbindValueElement(VisualElement element, int index)
        {
            var valueElement = element as ValueReferenceValueElement;

            valueElement.CleanUp();
        }

        private VisualElement CreateExtraControlsElement()
        {
            var parent = _valueReferenceExtraControlsElementUI.CloneTree();

            var element = parent.Q<ValueReferenceExtraControlsElement>();

            element.Initialize(OnElementRemoved);

            parent.Remove(element);

            return element;
        }

        private void BindExtraControlsElement(VisualElement element, int index)
        {
            var extraControlsElement = element as ValueReferenceExtraControlsElement;

            var data = _itemsSource[index];

            extraControlsElement.SetData(data, _groupAsset);
        }

        private void UnbindExtraControlsElement(VisualElement element, int index)
        {
            var extraControlsElement = element as ValueReferenceExtraControlsElement;

            extraControlsElement.CleanUp();
        }

        private void PingAsset()
        {
            EditorGUIUtility.PingObject(_groupAsset);
        }

        private bool IsOrphanGroup() => _pathProperty.stringValue == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME;

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
            newName = newName.Trim();

            if(_groupAsset.name == newName)
                return;

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

            _pingButton.style.display = DisplayStyle.None;
            _pingButton.enabledSelf = false;

            _addButton.style.display = DisplayStyle.None;
            _addButton.enabledSelf = false;

            _pathField.style.display = DisplayStyle.None;
            _pathField.enabledSelf = false;

            _itemsContainer.dragAndDropUpdate -= OnDragAndDropUpdate;
            _itemsContainer.handleDrop -= OnHandleDrop;
        }

        private void Refresh()
        {
            _itemsSource.Clear();
            serializedObject.Update();

            _itemsSource.AddRange(_groupAsset.ValueReferenceAssets);
            _itemsContainer.RefreshItems();
        }
    }
}
