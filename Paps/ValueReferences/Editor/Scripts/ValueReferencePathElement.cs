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
        [UxmlAttribute] private Texture2D _foldoutOpenedTexture;
        [UxmlAttribute] private Texture2D _foldoutClosedTexture;

        private string _pathNodeName;

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

        public void Initialize(string pathNodeName, ValueReferenceGroupAsset[] groupAssets, 
            ValueReferencePathElement[] childPathElements)
        {
            _groupEditors = new List<EditorObject>();
            _groupEditorElements = new List<VisualElement>();

            _pathNodeName = pathNodeName;
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

            _pathLabel.text = _pathNodeName;
            
            _expandCollapseButton.clicked += ExpandOrCollapse;
        }

        private void ExpandOrCollapse()
        {
            if(_foldoutContainer.style.display == DisplayStyle.Flex)
            {
                _foldoutContainer.style.display = DisplayStyle.None;
                _foldoutImage.image = _foldoutClosedTexture;
            }
            else
            {
                _foldoutContainer.style.display = DisplayStyle.Flex;
                _foldoutImage.image = _foldoutOpenedTexture;
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
    }
}
