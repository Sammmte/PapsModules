using Paps.LevelSetup;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [UxmlElement]
    public partial class LevelElement : VisualElement
    {
        private ScriptableLevel _level;
        private LevelEditorData _levelEditorData;

        private Label _levelNameLabel;
        private Button _openInitialScenesButton;
        private Button _openAllScenesButton;

        public LevelElement() { }

        public void Initialize(ScriptableLevel level, LevelEditorData levelEditorData)
        {
            _level = level;
            _levelEditorData = levelEditorData;

            _levelNameLabel = this.Q<Label>("LevelNameLabel");
            _openInitialScenesButton = this.Q<Button>("OpenInitialScenesButton");
            _openAllScenesButton = this.Q<Button>("OpenAllScenesButton");

            _levelNameLabel.text = level.Name;
            _openInitialScenesButton.clicked += OpenInitialScenes;
            _openAllScenesButton.clicked += OpenAllScenes;
        }

        private void OpenInitialScenes()
        {
            EditorLevelManager.LoadLevel(_level, null);
        }

        private void OpenAllScenes()
        {
            EditorLevelManager.LoadLevel(_level, _levelEditorData);
        }
    }
}