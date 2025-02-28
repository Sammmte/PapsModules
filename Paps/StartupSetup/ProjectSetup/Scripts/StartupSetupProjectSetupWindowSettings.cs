using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.StartupSetup.ProjectSetup
{
    public class StartupSetupProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Startup Setup";

        private VisualElement _container;

        public StartupSetupProjectSetupWindowSettings()
        {
            _container = new VisualElement();

            var label = new Label("A Setup scene will be created at Assets/Game/Setup along with a default game setup pipeline");
            label.style.whiteSpace = WhiteSpace.Normal;

            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new StartupSetupProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
