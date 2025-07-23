using Cysharp.Threading.Tasks;
using Paps.SceneLoading;
using Paps.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.LevelSetup
{
    public static class LevelSetupper
    {
        private const int DEFAULT_OBJECTS_CAPACITY = 1000;
        private const int DEFAULT_SCENES_CAPACITY = 50;
        
        private static Level _currentLevel;
        private static List<ILevelSetuppable> _everPresentLevelSetuppables = new List<ILevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);
        private static List<Scene> _levelScenes = new List<Scene>(DEFAULT_SCENES_CAPACITY);
        private static List<GameObject> _rootGameObjectsList = new List<GameObject>(DEFAULT_OBJECTS_CAPACITY);
        private static List<SceneBoundLevelSetuppable> _sceneBound = new List<SceneBoundLevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);
        private static List<SceneBoundLevelSetuppable> _tempSceneBoundBuffer = new List<SceneBoundLevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);
        private static List<ILevelSetuppable> _tempLevelSetuppableBuffer = new List<ILevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);

        public static async UniTask LoadAndSetupInitialLevel(Level level)
        {
            var loadedScenes = SceneLoader.GetLoadedScenes();

            await SceneLoader.LoadNewSceneAndWaitOneFrame("LevelSetup_EmptyScene", LoadSceneMode.Additive);
            await SceneLoader.UnloadAsync(loadedScenes);
            await Load(level);
        }

        public static async UniTask LoadAndSetupLevel(Level level, Func<UniTask> onUnload = null)
        {
            await Unload();

            if (onUnload != null)
                await onUnload();

            await Load(level);
        }

        public static async UniTask LoadScenesToLevel(SceneGroup scenes)
        {
            await SceneLoader.LoadAsync(scenes, LoadSceneMode.Additive);

            _levelScenes.AddRange(scenes.Scenes);

            await UniTask.NextFrame();

            await SetupAndKickstartFrom(scenes, false);
        }

        public static async UniTask UnloadScenesFromLevel(SceneGroup scenes)
        {
            await UnloadFrom(scenes, false);

            await SceneLoader.UnloadAsync(scenes);

            foreach(var sceneName in scenes.Scenes)
                _levelScenes.Remove(sceneName);
        }

        private static async UniTask Load(Level level)
        {
            await SceneLoader.LoadAsync(level.InitialScenesGroup, LoadSceneMode.Additive);

            _currentLevel = level;
            _levelScenes.AddRange(level.InitialScenesGroup.Scenes);

            SceneLoader.SetActiveScene(level.ActiveScene);

            await UniTask.NextFrame();

            await SetupAndKickstartFrom(level.InitialScenesGroup, true);
        }

        private static async UniTask Unload(Func<UniTask> onUnload = null)
        {
            var sceneGroup = new SceneGroup(_levelScenes.ToArray());

            await UnloadFrom(sceneGroup, true);

            await SceneLoader.UnloadAsync(sceneGroup);

            _levelScenes.Clear();

            if (onUnload != null)
                await onUnload();
        }

        public static void RegisterEverPresent(ILevelSetuppable levelSetuppable)
        {
            if(_everPresentLevelSetuppables.Contains(levelSetuppable))
                return;
            
            _everPresentLevelSetuppables.Add(levelSetuppable);
        }

        private static async UniTask SetupAndKickstartFrom(SceneGroup sceneGroup, bool includeEverPresent)
        {
            for (int i = 0; i < sceneGroup.Scenes.Length; i++)
            {
                var scene = sceneGroup.Scenes[i];
                scene.GetRootGameObjects(_rootGameObjectsList);

                for (int j = 0; j < _rootGameObjectsList.Count; j++)
                {
                    _rootGameObjectsList[j].GetComponentsInChildren(true, _tempLevelSetuppableBuffer);

                    for (int k = 0; k < _tempLevelSetuppableBuffer.Count; k++)
                    {
                        _tempSceneBoundBuffer.Add(new SceneBoundLevelSetuppable(_tempLevelSetuppableBuffer[k], scene));
                    }
                    
                    _tempLevelSetuppableBuffer.Clear();
                }
                
                _rootGameObjectsList.Clear();
            }

            if (includeEverPresent)
                await UniTask.WhenAll(_everPresentLevelSetuppables.ToArray(l => l.Setup()));

            await UniTask.WhenAll(_tempSceneBoundBuffer.ToArray(l => l.LevelSetuppable.Setup()));

            if (includeEverPresent)
            {
                foreach (var levelSetuppable in _everPresentLevelSetuppables)
                {
                    levelSetuppable.Kickstart();
                }
            }

            for (int i = 0; i < _tempSceneBoundBuffer.Count; i++)
            {
                _tempSceneBoundBuffer[i].LevelSetuppable.Kickstart();
            }
            
            _sceneBound.AddRange(_tempSceneBoundBuffer);
            
            _tempSceneBoundBuffer.Clear();
        }

        private static async UniTask UnloadFrom(SceneGroup sceneGroup, bool includeEverPresent)
        {
            for (int i = 0; i < _sceneBound.Count; i++)
            {
                if (sceneGroup.Scenes.Contains(_sceneBound[i].Scene))
                {
                    _tempSceneBoundBuffer.Add(_sceneBound[i]);
                }
            }

            await UniTask.WhenAll(_tempSceneBoundBuffer.ToArray(l => l.LevelSetuppable.Unload()));
            
            if (includeEverPresent)
                await UniTask.WhenAll(_everPresentLevelSetuppables.ToArray(l => l.Unload()));

            for (int i = 0; i < _tempSceneBoundBuffer.Count; i++)
                _sceneBound.Remove(_tempSceneBoundBuffer[i]);
            
            _tempSceneBoundBuffer.Clear();
        }
    }
}
