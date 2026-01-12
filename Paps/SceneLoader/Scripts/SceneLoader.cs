using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paps.SceneLoading
{
    public static class SceneLoader
    {
        public static void Load(ReadOnlySpan<Scene> sceneGroup, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!IsValidSceneGroup(ref sceneGroup))
                return;

            SceneManager.LoadScene(sceneGroup[0].Name, loadSceneMode);

            for(int i = 1; i < sceneGroup.Length; i++)
            {
                SceneManager.LoadScene(sceneGroup[i].Name, LoadSceneMode.Additive);
            }
        }

        public static Scene LoadNewScene(string newSceneName)
        {
            return SceneManager.CreateScene(newSceneName);
        }

        public static async UniTask<Scene> LoadNewSceneAndWaitOneFrame(string newSceneName)
        {
            var scene = LoadNewScene(newSceneName);
            await UniTask.NextFrame();
            return scene;
        }

        public static async UniTask LoadAsync(ReadOnlyMemory<Scene> sceneGroup, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!IsValidSceneGroup(ref sceneGroup))
                return;

            await SceneManager.LoadSceneAsync(sceneGroup.Span[0].Name, loadSceneMode).ToUniTask();

            for (int i = 1; i < sceneGroup.Length; i++)
            {
                await SceneManager.LoadSceneAsync(sceneGroup.Span[i].Name, LoadSceneMode.Additive).ToUniTask();
            }
        }

        public static async UniTask LoadAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode).ToUniTask();
        }

        public static async UniTask UnloadAsync(ReadOnlyMemory<Scene> sceneGroup)
        {
            if (!IsValidSceneGroup(ref sceneGroup))
                return;

            var tasks = new UniTask[sceneGroup.Length];
            
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = SceneManager.UnloadSceneAsync(sceneGroup.Span[i].Name).ToUniTask();
            } 

            await UniTask.WhenAll(tasks);
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

        private static bool IsValidSceneGroup(ref ReadOnlySpan<Scene> sceneGroup)
        {
            return sceneGroup.Length > 0;
        }

        private static bool IsValidSceneGroup(ref ReadOnlyMemory<Scene> sceneGroup)
        {
            return sceneGroup.Length > 0;
        }

        public static void MoveGameObjectToScene(GameObject gameObject, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(scene.Name));
        }
    }
}
