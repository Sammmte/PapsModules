using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.Localization.ProjectSetup
{
    public class LocalizationProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Localization";

        private VisualElement _container;

        public LocalizationProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("Will create a game setup asset on Assets/Game/Localization. Remember to setup Unity's localization settings");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new LocalizationProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
