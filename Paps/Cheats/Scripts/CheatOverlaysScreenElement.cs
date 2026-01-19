using System.Collections.Generic;
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

            container.SetOverlayElement(overlay.GetVisualElement());

            _overlayContainers[overlay] = container;

            _itemsContainer.Add(container);
        }

        public void RemoveOverlay(ICheatOverlay overlay)
        {
            if(_overlayContainers.TryGetValue(overlay, out var container))
            {
                _elementContainerPool.Release(container);
            }
        }
    }
}
