using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.InGameDebugConsole.ProjectSetup
{
    public class InGameDebugConsoleProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "In Game Debug Console";

        private VisualElement _container;

        public InGameDebugConsoleProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("InGameDebugConsole cheats");
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new InGameDebugConsoleProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
