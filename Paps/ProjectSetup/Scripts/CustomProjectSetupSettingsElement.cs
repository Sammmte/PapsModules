using UnityEngine.UIElements;

namespace Paps.ProjectSetup
{
    [UxmlElement]
    public partial class CustomProjectSetupSettingsElement : VisualElement
    {
        private IProjectSetupWindowSettings _projectSetupWindowSettings;

        private Label _titleLabel;
        private VisualElement _settingsContainer;

        public void Initialize(IProjectSetupWindowSettings projectSetupWindowSettings)
        {
            _projectSetupWindowSettings = projectSetupWindowSettings;

            _titleLabel = this.Q<Label>("Title");
            _settingsContainer = this.Q("SettingsContainer");

            _titleLabel.text = _projectSetupWindowSettings.Title;
            _settingsContainer.Add(_projectSetupWindowSettings.GetVisualElement());
        }

        public object GetSettings()
        {
            return _projectSetupWindowSettings.GetSettingsObject();
        }
    }
}