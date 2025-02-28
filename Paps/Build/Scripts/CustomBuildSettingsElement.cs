using UnityEngine.UIElements;

namespace Paps.Build
{
    [UxmlElement]
    public partial class CustomBuildSettingsElement : VisualElement
    {
        private IBuildWindowSettings _buildWindowSettings;

        private Label _titleLabel;
        private VisualElement _settingsContainer;

        public void Initialize(IBuildWindowSettings buildWindowSettings)
        {
            _buildWindowSettings = buildWindowSettings;

            _titleLabel = this.Q<Label>("Title");
            _settingsContainer = this.Q("SettingsContainer");

            _titleLabel.text = _buildWindowSettings.Title;
            _settingsContainer.Add(_buildWindowSettings.GetVisualElement());
        }

        public object GetSettings()
        {
            return _buildWindowSettings.GetSettingsObject();
        }
    }
}