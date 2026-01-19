using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    [UxmlElement]
    public partial class CheatOverlaysScreenElement : VisualElement
    {
        private VisualElement _itemsContainer;
        private VisualTreeAsset _elementContainerVTA;

        private ObjectPool<CheatOverlayElementContainer> _elementContainerPool;

        private Dictionary<ICheatOverlay, CheatOverlayElementContainer> _overlayContainers;

        public event Action<ICheatOverlay, Vector2> OnOverlayPositionChanged;

        public void Initialize(VisualTreeAsset elementContainerVTA)
        {
            _elementContainerVTA = elementContainerVTA;

            _overlayContainers = new Dictionary<ICheatOverlay, CheatOverlayElementContainer>();
            _elementContainerPool = new ObjectPool<CheatOverlayElementContainer>(CreateContainer, actionOnRelease: OnRelease);
            _itemsContainer = this.Q("OverlaysContainer");
        }

        private CheatOverlayElementContainer CreateContainer()
        {
            var parent = _elementContainerVTA.CloneTree();

            var container = parent.Q<CheatOverlayElementContainer>();

            parent.Remove(container);

            container.Initialize();

            return container;
        }

        private void OnRelease(CheatOverlayElementContainer container)
        {
            container.CleanUp();
        }

        public void AddOverlay(ICheatOverlay overlay)
        {
            var container = _elementContainerPool.Get();

            container.OnPositionChanged += OnElementPositionChanged;
            container.SetOverlayElement(overlay, overlay.GetVisualElement());

            _overlayContainers[overlay] = container;

            _itemsContainer.Add(container);
        }

        public void RemoveOverlay(ICheatOverlay overlay)
        {
            if(_overlayContainers.TryGetValue(overlay, out var container))
            {
                container.OnPositionChanged -= OnElementPositionChanged;
                _elementContainerPool.Release(container);
            }
        }

        private void OnElementPositionChanged(ICheatOverlay overlay, Vector2 newPosition)
        {
            OnOverlayPositionChanged?.Invoke(overlay, newPosition);
        }

        public void SetOverlayPosition(ICheatOverlay overlay, Vector2 position)
        {
            var container = _overlayContainers[overlay];

            container.SetPosition(position);
        }
    }
}
