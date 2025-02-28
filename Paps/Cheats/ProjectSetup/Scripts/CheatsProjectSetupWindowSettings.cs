using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.Cheats.ProjectSetup
{
    public class CheatsProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Cheats";

        private VisualElement _container;

        public CheatsProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("Will add cheats module assets to addressables Cheats group. It is recommended that any other cheat assets go in the same group to strip them automatically from build");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new CheatsProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
