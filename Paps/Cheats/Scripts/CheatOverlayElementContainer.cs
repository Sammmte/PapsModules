using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    [UxmlElement]
    public partial class CheatOverlayElementContainer : VisualElement
    {
        private DragManipulator _dragManipulator;
        private VisualElement _overlayContainer;

        private VisualElement _overlayElement;
        private ICheatOverlay _overlay;

        public event Action<ICheatOverlay, Vector2> OnPositionChanged;

        public void Initialize()
        {
            _dragManipulator = new DragManipulator();

            _dragManipulator.OnPositionChanged += OnDragPositionChanged;

            this.AddManipulator(_dragManipulator);

            _overlayContainer = this.Q("OverlayContainer");
        }

        private void OnDragPositionChanged(Vector2 newPosition)
        {
            OnPositionChanged?.Invoke(_overlay, newPosition);
        }

        public void SetOverlayElement(ICheatOverlay overlay, VisualElement element)
        {
            CleanUp();

            _overlay = overlay;
            _overlayElement = element;

            _overlayContainer.Add(_overlayElement);

            style.display = DisplayStyle.Flex;

            enabledSelf = true;
        }

        public void CleanUp()
        {
            _overlayContainer.Clear();

            _overlay = null;
            _overlayElement = null;

            style.display = DisplayStyle.None;

            enabledSelf = false;
        }

        public void SetPosition(Vector2 position)
        {
            _dragManipulator.SetPosition(position);
        }
    }
}
