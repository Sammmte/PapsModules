using Paps.ProjectSetup;
using UnityEngine.UIElements;

namespace Paps.Entry.ProjectSetup
{
    public class EntryProjectSetupWindowSettings : IProjectSetupWindowSettings
    {
        public string Title => "Entry";

        private VisualElement _container;

        public EntryProjectSetupWindowSettings()
        {
            _container = new VisualElement();
            var label = new Label("An Entry scene will be created at Assets/Game/Entry. Remember to add an initial level on Entry object");
            label.style.whiteSpace = WhiteSpace.Normal;
            _container.Add(label);
        }

        public object GetSettingsObject()
        {
            return new EntryProjectSetupSettings();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
