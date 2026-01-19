using System;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class CheatOverlayVisibilityElement : VisualElement
    {
        private const string OVERLAY_VISIBILITY_ELEMENT_CLASS = "cheat-overlays-submenu__item__visibility-toggle";

        private Toggle _toggle;

        private ICheatOverlay _currentOverlay;

        public event Action<ICheatOverlay, bool> OnValueChanged;

        public void Initialize()
        {
            _toggle = new Toggle();
            _toggle.RegisterValueChangedCallback(ev => HandleValueChanged(ev.newValue));
            _toggle.AddToClassList(OVERLAY_VISIBILITY_ELEMENT_CLASS);

            Add(_toggle);
        }

        public void SetData(ICheatOverlay cheatOverlay)
        {
            CleanUp();

            _currentOverlay = cheatOverlay;
        }

        public void CleanUp()
        {
            _currentOverlay = null;
        }

        private void HandleValueChanged(bool newValue)
        {
            OnValueChanged?.Invoke(_currentOverlay, newValue);
        }
    }
}
