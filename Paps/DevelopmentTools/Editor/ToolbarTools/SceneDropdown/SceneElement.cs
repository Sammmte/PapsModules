using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [UxmlElement]
    public partial class SceneElement : VisualElement
    {
        private string _scenePath;

        private Label _sceneNameLabel;
        private Button _goToSceneButton;

        public SceneElement() { }

        public void Initialize(string scenePath)
        {
            _scenePath = scenePath;

            _sceneNameLabel = this.Q<Label>("SceneNameLabel");
            _goToSceneButton = this.Q<Button>("GoToSceneButton");

            _sceneNameLabel.text = Path.GetFileNameWithoutExtension(_scenePath);
            _goToSceneButton.clicked += GoToScene;
        }

        private void GoToScene()
        {
            EditorSceneManager.OpenScene(_scenePath);
        }
    }
}