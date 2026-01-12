using Paps.LevelSetup;
using UnityEngine.UIElements;

namespace Paps.LevelSetup.Editor
{
    [UxmlElement]
    public partial class LevelElement : VisualElement
    {
        private Level _level;

        private Label _levelNameLabel;
        private Button _openInitialScenesButton;
        private Button _openAllScenesButton;

        public LevelElement() { }

        public void Initialize(Level level)
        {
            _level = level;

            _levelNameLabel = this.Q<Label>("LevelNameLabel");
            _openInitialScenesButton = this.Q<Button>("OpenInitialScenesButton");
            _openAllScenesButton = this.Q<Button>("OpenAllScenesButton");

            _levelNameLabel.text = level.Id;
            _levelNameLabel.tooltip = level.Id;
            _openInitialScenesButton.clicked += OpenInitialScenes;
            _openAllScenesButton.clicked += OpenAllScenes;
        }

        private void OpenInitialScenes()
        {
            EditorLevelManager.LoadLevel(_level);
        }

        private void OpenAllScenes()
        {
            EditorLevelManager.LoadLevel(_level, true);
        }
    }
}