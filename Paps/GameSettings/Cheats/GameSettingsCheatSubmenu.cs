using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.GameSettings.Cheats
{
    public class GameSettingsCheatSubmenu : ICheatSubmenu
    {
        public string DisplayName => "Game Settings";

        private VisualElement _container;
        private Label _currentResolutionLabel;
        private TextField _widthField, _heightField;
        private Button _changeResolutionButton;
        private Label _currentFrameRateLabel;
        private TextField _frameRateField;
        private Button _changeFrameRateButton;
        private Toggle _vsyncToggle;

        public VisualElement GetVisualElement()
        {
            return _container;
        }

        public async UniTask Load()
        {
            _container = new VisualElement();

            _currentResolutionLabel = new Label();

            _widthField = new TextField("Width");
            _heightField = new TextField("Height");

            _changeResolutionButton = new Button();
            _changeResolutionButton.text = "Change Resolution";
            _changeResolutionButton.clicked += ChangeResolution;

            _currentFrameRateLabel = new Label();

            _frameRateField = new TextField("Frame Rate");

            _changeFrameRateButton = new Button();
            _changeFrameRateButton.text = "Change Frame Rate";
            _changeFrameRateButton.clicked += ChangeFrameRate;

            _vsyncToggle = new Toggle("VSync");

            _container.Add(_currentResolutionLabel);
            _container.Add(_widthField);
            _container.Add(_heightField);
            _container.Add(_changeResolutionButton);
            _container.Add(_currentFrameRateLabel);
            _container.Add(_frameRateField);
            _container.Add(_changeFrameRateButton);

            UpdateResolutionLabel();
            UpdateFrameRateLabel();
        }

        private void ChangeResolution()
        {
            var width = _widthField.GetInt(Screen.width);
            var height = _heightField.GetInt(Screen.height);

            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);

            UpdateResolutionLabel();
        }

        private void ChangeFrameRate()
        {
            var frameRate = _frameRateField.GetInt(Application.targetFrameRate);

            Application.targetFrameRate = frameRate;

            UpdateFrameRateLabel();
        }

        private void UpdateResolutionLabel()
        {
            _currentResolutionLabel.text = $"Current Resolution: {Screen.width}x{Screen.height}";
        }

        private void UpdateFrameRateLabel()
        {
            _currentFrameRateLabel.text = $"Current Frame Rate: {Application.targetFrameRate}";
        }
    }
}
