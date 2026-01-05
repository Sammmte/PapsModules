using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceGroupElement : VisualElement, IDisposable
    {
        private ValueReferenceGroupAsset _groupAsset;

        private VisualElement _valueReferencesContainer;
        private ValueReferenceElement[] _valueReferenceElements;

        private Label _groupNameLabel;

        public void Initialize(ValueReferenceGroupAsset groupAsset, VisualTreeAsset valueReferenceTreeAsset)
        {
            _groupAsset = groupAsset;
            _valueReferenceElements = _groupAsset.ValueReferenceAssets.Select(asset =>
            {
                var valueReferenceElementParent = valueReferenceTreeAsset.CloneTree();
                var valueReferenceElement = valueReferenceElementParent.Q<ValueReferenceElement>();

                valueReferenceElement.Initialize(asset);

                return valueReferenceElement;
            }).ToArray();

            _valueReferencesContainer = this.Q("ValueReferenceElementsContainer");

            _groupNameLabel = this.Q<Label>("GroupNameLabel");

            _groupNameLabel.text = groupAsset.name;

            foreach(var element in _valueReferenceElements)
            {
                _valueReferencesContainer.Add(element);
            }
        }

        public void Dispose()
        {
            for(int i = 0; i < _valueReferenceElements.Length; i++)
            {
                _valueReferenceElements[i].Dispose();
            }
        }
    }
}
