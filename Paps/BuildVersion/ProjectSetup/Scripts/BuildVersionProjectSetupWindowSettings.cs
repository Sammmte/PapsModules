using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.BuildVersion.ProjectSetup
{
    public class BuildVersionProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Build Version";

        private VisualElement _container;

        public BuildVersionProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("Will add BuildVersionMenu prefab to addressables BuildInfo group");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new BuildVersionProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
