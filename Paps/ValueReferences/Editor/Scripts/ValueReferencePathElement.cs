using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferencePathElement : VisualElement, IDisposable
    {
        private const string BASE_KEY = "value-reference-path-element";
        private const string FOLDOUT_STATE_KEY = "foldout-state";

        [UxmlAttribute] private Texture2D _foldoutOpenedTexture;
        [UxmlAttribute] private Texture2D _foldoutClosedTexture;

        private TreeNode<ValueReferencePathElement> _treeNode;

        private ValueReferenceGroupAsset[] _groupAssets;
        private ValueReferencePathElement[] _childPathElements;
        
        private VisualElement _groupElementsContainer;
        private VisualElement _childPathElementsContainer;

        private Label _pathLabel;
        private Button _expandCollapseButton;
        private VisualElement _foldoutContainer;
        private Image _foldoutImage;

        private List<EditorObject> _groupEditors;
        private List<VisualElement> _groupEditorElements;

        public ValueReferencePathElement[] ChildPathElements => _childPathElements;

        public void Initialize(TreeNode<ValueReferencePathElement> treeNode, ValueReferenceGroupAsset[] groupAssets, 
            ValueReferencePathElement[] childPathElements)
        {
            _treeNode = treeNode;
            _groupEditors = new List<EditorObject>();
            _groupEditorElements = new List<VisualElement>();

            _groupAssets = groupAssets;
            _childPathElements = childPathElements;

            _groupElementsContainer = this.Q("GroupElementsContainer");
            _childPathElementsContainer = this.Q("ChildPathElementsContainer");

            _pathLabel = this.Q<Label>("PathNodeNameLabel");
            _expandCollapseButton = this.Q<Button>("FoldoutButton");
            _foldoutContainer = this.Q("FoldoutContainer");
            _foldoutImage = this.Q<Image>("FoldoutArrow");

            foreach(var groupAsset in _groupAssets)
            {
                var editor = EditorObject.CreateEditor(groupAsset);

                var editorVisualElement = editor.CreateInspectorGUI();

                _groupEditors.Add(editor);
                _groupEditorElements.Add(editorVisualElement);

                _groupElementsContainer.Add(editorVisualElement);
            }

            foreach(var childPathElement in _childPathElements)
            {
                _childPathElementsContainer.Add(childPathElement);
            }

            _pathLabel.text = GetNodeName(_treeNode.Name);
            
            _expandCollapseButton.clicked += OnExpandCollapseButtonClicked;

            LoadState();
        }

        private void OnExpandCollapseButtonClicked()
        {
            var newState = SwitchExpandOrCollapse();
            ValueReferencesUIUtils.VALUE_REFERENCES_EDITOR_USER_PROJECT_PREFS.Set(CreateSaveKey(FOLDOUT_STATE_KEY), newState);
        }

        private bool SwitchExpandOrCollapse()
        {
            var expand = _foldoutContainer.style.display != DisplayStyle.Flex;

            ExpandOrCollapse(expand);

            return expand;
        }

        private void ExpandOrCollapse(bool expand)
        {
            if(expand)
            {
                _foldoutContainer.style.display = DisplayStyle.Flex;
                _foldoutImage.image = _foldoutOpenedTexture;
            }
            else
            {
                _foldoutContainer.style.display = DisplayStyle.None;
                _foldoutImage.image = _foldoutClosedTexture;
            }
        }

        public void Dispose()
        {
            foreach(var editor in _groupEditors)
            {
                EditorObject.DestroyImmediate(editor);
            }

            _groupEditors.Clear();
            _groupEditorElements.Clear();
        }

        private void LoadState()
        {
            var shouldExpand = ValueReferencesUIUtils.VALUE_REFERENCES_EDITOR_USER_PROJECT_PREFS.Get<bool>(CreateSaveKey(FOLDOUT_STATE_KEY));

            ExpandOrCollapse(shouldExpand);
        }

        private string CreateSaveKey(string subKey)
        {
            return $"{BASE_KEY}:{_treeNode.GetPath()}:{subKey}";
        }

        private string GetNodeName(string inputName)
        {
            if(inputName == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME)
                return "ORPHAN VALUES";

            return inputName;
        }
    }
}
