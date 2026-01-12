using Cysharp.Threading.Tasks;
using Paps.SceneLoading;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.ScreenTransitions
{
    public class BlackScreenTransition : ScriptableObject, IScreenTransition
    {
        [SerializeField] private UIDocument _uiDocumentPrefab;

        private Scene _scene;
        private VisualElement _blackScreen;

        public async UniTask PlayIn()
        {
            CleanUp();

            _scene = SceneLoader.LoadNewScene("TransitionScene");

            await UniTask.NextFrame();

            var prefabInstance = GameObject.Instantiate(_uiDocumentPrefab);
            SceneLoader.MoveGameObjectToScene(prefabInstance.gameObject, _scene);

            _blackScreen = prefabInstance.rootVisualElement.Q("BlackScreen");

            await Tween.VisualElementBackgroundColor(_blackScreen, endValue: Color.black, duration: 1);
        }

        public async UniTask PlayOut()
        {
            var color = Color.black;
            color.a = 0;

            await Tween.VisualElementBackgroundColor(_blackScreen, endValue: color, duration: 1);

            await SceneLoader.UnloadAsync(_scene);

            CleanUp();
        }

        private void CleanUp()
        {
            _blackScreen = null;
            _scene = default;
        }
    }
}
