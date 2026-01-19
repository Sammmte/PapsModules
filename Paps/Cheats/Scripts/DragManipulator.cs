using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class DragManipulator : PointerManipulator
    {
        private bool _dragging;
        private Vector2 _startDragOffset;

        public DragManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (CanStartManipulation(evt))
            {
                _dragging = true;
                _startDragOffset = ((Vector2)evt.position) - target.layout.position;
                target.CapturePointer(evt.pointerId);
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_dragging && target.HasPointerCapture(evt.pointerId))
            {
                Vector2 newPosition = ((Vector2)evt.position) - _startDragOffset;

                newPosition = ClampToScreen(newPosition);

                target.style.left = newPosition.x;
                target.style.top = newPosition.y;
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (_dragging && target.HasPointerCapture(evt.pointerId))
            {
                _dragging = false;
                target.ReleasePointer(evt.pointerId);
            }
        }

        Vector2 ClampToScreen(Vector2 position)
        {
            float maxX = target.panel.visualTree.layout.width - target.layout.size.x;
            float maxY = target.panel.visualTree.layout.height - target.layout.size.y;

            position.x = Mathf.Clamp(position.x, 0, maxX);
            position.y = Mathf.Clamp(position.y, 0, maxY);

            return position;
        }
    }
}
