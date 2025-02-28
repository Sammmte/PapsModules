using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.FPSMonitor.ProjectSetup
{
    public class FPSMonitorProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "FPS Monitor";

        private VisualElement _container;

        public FPSMonitorProjectSetupWindowSettings()
        {
            _container = new VisualElement();

            var label = new Label("FPS Monitor cheats");

            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new FPSMonitorProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
