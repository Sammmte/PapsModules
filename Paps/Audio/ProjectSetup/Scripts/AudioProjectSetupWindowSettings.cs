using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.Audio.ProjectSetup
{
    public class AudioProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Audio";

        private VisualElement _container;

        public AudioProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("Will create an audio setup asset with a default audio mixer. Change it later if you want/need");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new AudioProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
