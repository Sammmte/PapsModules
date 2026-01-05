using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
        private ValueReferenceGroupElement[] _groupElements;

        private Label _pathLabel;
        private Button _expandCollapseButton;
        private VisualElement _foldoutContainer;
        private Image _foldoutImage;

        public void Initialize(string pathNodeName, ValueReferenceGroupAsset[] groupAssets, ValueReferencePathElement[] childPathElements,
            VisualTreeAsset groupElementTreeAsset, VisualTreeAsset valueReferenceTreeAsset)
        {
            _pathNodeName = pathNodeName;
            _groupAssets = groupAssets;
            _childPathElements = childPathElements;
            _groupElements = _groupAssets.Select(asset =>
            {
                var groupElementParent = groupElementTreeAsset.CloneTree();
                var groupElement = groupElementParent.Q<ValueReferenceGroupElement>();

                groupElement.Initialize(asset, valueReferenceTreeAsset);

                return groupElement;
            }).ToArray();

            _groupElementsContainer = this.Q("GroupElementsContainer");
            _childPathElementsContainer = this.Q("ChildPathElementsContainer");

            _pathLabel = this.Q<Label>("PathNodeNameLabel");
            _expandCollapseButton = this.Q<Button>("FoldoutButton");
            _foldoutContainer = this.Q("FoldoutContainer");
            _foldoutImage = this.Q<Image>("FoldoutArrow");

            foreach(var groupElement in _groupElements)
            {
                _groupElementsContainer.Add(groupElement);
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
            for(int i = 0; i < _groupElements.Length; i++)
            {
                _groupElements[i].Dispose();
            }
        }
    }
}
