using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.Logging.ProjectSetup
{
    public class LoggingProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Logging";

        private VisualElement _container;

        public LoggingProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("Will create a logging configuration at Assets/Game/Logging and add it to addressables Logging group");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new LoggingProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
