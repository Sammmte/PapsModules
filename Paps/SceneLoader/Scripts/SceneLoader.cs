using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paps.SceneLoading
{
    public static class SceneLoader
    {
        public static void Load(SceneGroup sceneGroup, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!IsValidSceneGroup(sceneGroup))
                return;

            SceneManager.LoadScene(sceneGroup.Scenes[0].Name, loadSceneMode);

            for(int i = 1; i < sceneGroup.Scenes.Length; i++)
            {
                SceneManager.LoadScene(sceneGroup.Scenes[i].Name, LoadSceneMode.Additive);
            }
        }

        public static Scene LoadNewScene(string newSceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            return SceneManager.CreateScene(newSceneName);
        }

        public static async UniTask<Scene> LoadNewSceneAndWaitOneFrame(string newSceneName, 
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var scene = LoadNewScene(newSceneName, loadSceneMode);
            await UniTask.NextFrame();
            return scene;
        }

        public static async UniTask LoadAsync(SceneGroup sceneGroup, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!IsValidSceneGroup(sceneGroup))
                return;

            await SceneManager.LoadSceneAsync(sceneGroup.Scenes[0].Name, loadSceneMode).ToUniTask();

            for (int i = 1; i < sceneGroup.Scenes.Length; i++)
            {
                await SceneManager.LoadSceneAsync(sceneGroup.Scenes[i].Name, LoadSceneMode.Additive).ToUniTask();
            }
        }

        public static async UniTask LoadAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode).ToUniTask();
        }

        public static async UniTask UnloadAsync(SceneGroup sceneGroup)
        {
            if (!IsValidSceneGroup(sceneGroup))
                return;

            await UniTask.WhenAll(
                sceneGroup.Scenes.Select(scene => SceneManager.UnloadSceneAsync(scene.Name).ToUniTask())
                );
        }

        public static async UniTask UnloadAsync(params Scene[] scenes)
        {
            await UniTask.WhenAll(
                scenes.Select(scene => SceneManager.UnloadSceneAsync(scene.Name).ToUniTask())
                );
        }

        public static async UniTask UnloadAsync(Scene scene)
        {
            await SceneManager.UnloadSceneAsync(scene.Name).ToUniTask();
        }

        public static void SetActiveScene(string sceneName) => SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        public static void SetActiveScene(Scene scene) => SetActiveScene(scene.Name);

        public static Scene[] GetLoadedScenes()
        {
            var scenes = new Scene[SceneManager.sceneCount];

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes[i] = SceneManager.GetSceneAt(i);
            }

            return scenes;
        }

        private static bool IsValidSceneGroup(SceneGroup sceneGroup)
        {
            return sceneGroup.Scenes != null && sceneGroup.Scenes.Length > 0;
        }

        public static void MoveGameObjectToScene(GameObject gameObject, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(scene.Name));
        }
    }
}
