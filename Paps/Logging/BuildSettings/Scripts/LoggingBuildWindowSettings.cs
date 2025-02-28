using Paps.Build;
using UnityEngine.UIElements;

namespace Paps.Logging.Build
{
    public class LoggingBuildWindowSettings : IBuildWindowSettings
    {
        public string Title => "Logging";

        private VisualElement _container;
        private Toggle _logEnabledToggle;

        public LoggingBuildWindowSettings()
        {
            _container = new VisualElement();
            _logEnabledToggle = new Toggle("Enabled");

            _container.Add(_logEnabledToggle);
        }

        public object GetSettingsObject()
        {
            return new LoggingBuildSettings()
            {
                LogEnabled = _logEnabledToggle.value
            };
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
