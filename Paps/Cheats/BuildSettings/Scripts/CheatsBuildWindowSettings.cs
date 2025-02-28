using Paps.Build;
using UnityEngine.UIElements;

namespace Paps.Cheats.Build
{
    public class CheatsBuildWindowSettings : IBuildWindowSettings
    {
        public string Title => "Cheats";

        private VisualElement _container;
        private Toggle _cheatsEnabledToggle;

        public CheatsBuildWindowSettings()
        {
            _container = new VisualElement();

            _cheatsEnabledToggle = new Toggle("Enabled");

            _container.Add(_cheatsEnabledToggle);
        }

        public object GetSettingsObject()
        {
            return new CheatsBuildSettings()
            {
                CheatsEnabled = _cheatsEnabledToggle.value
            };
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
