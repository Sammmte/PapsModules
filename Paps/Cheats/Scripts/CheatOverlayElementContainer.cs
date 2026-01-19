using UnityEngine.UIElements;

namespace Paps.Cheats
{
    [UxmlElement]
    public partial class CheatOverlayElementContainer : VisualElement
    {
        private DragManipulator _dragManipulator;
        private VisualElement _overlayContainer;

        private VisualElement _overlayElement;

        public void Initialize()
        {
            _dragManipulator = new DragManipulator();

            this.AddManipulator(_dragManipulator);

            _overlayContainer = this.Q("OverlayContainer");
        }

        public void SetOverlayElement(VisualElement element)
        {
            CleanUp();

            _overlayElement = element;

            _overlayContainer.Add(_overlayElement);

            style.display = DisplayStyle.Flex;

            enabledSelf = true;
        }

        public void CleanUp()
        {
            _overlayContainer.Clear();

            _overlayElement = null;

            style.display = DisplayStyle.None;

            enabledSelf = false;
        }
    }
}
