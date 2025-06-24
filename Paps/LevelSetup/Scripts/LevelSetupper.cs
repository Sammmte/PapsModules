using Cysharp.Threading.Tasks;
using Paps.SceneLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;
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
            var setuppablesPerScene = sceneGroup.Scenes.AsValueEnumerable()
                .Select(scene =>
                {
                    scene.GetRootGameObjects(_rootGameObjectsList);

                    var levelSetuppables = _rootGameObjectsList.AsValueEnumerable()
                        .SelectMany(g => g.DescendantsAndSelf())
                        .Select(g => g.GetComponent<ILevelSetuppable>())
                        .Where(l => l != null);

                    return (Scene: scene, LevelSetuppables: levelSetuppables);
                });
            
            _rootGameObjectsList.Clear();

            foreach (var item in setuppablesPerScene)
            {
                foreach (var levelSetuppable in item.LevelSetuppables)
                {
                    _tempSceneBoundBuffer.Add(new SceneBoundLevelSetuppable(levelSetuppable, item.Scene));
                }
            }

            if (includeEverPresent)
                await UniTask.WhenAll(_everPresentLevelSetuppables.AsValueEnumerable()
                    .Select(l => l.Setup()).ToArray());

            await UniTask.WhenAll(_tempSceneBoundBuffer.AsValueEnumerable()
                .Select(l => l.LevelSetuppable.Setup()).ToArray());

            if (includeEverPresent)
            {
                foreach (var levelSetuppable in _everPresentLevelSetuppables)
                {
                    levelSetuppable.Kickstart();
                }
            }
            
            foreach (var item in _tempSceneBoundBuffer.AsValueEnumerable())
            {
                item.LevelSetuppable.Kickstart();
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

            await UniTask.WhenAll(_tempSceneBoundBuffer.AsValueEnumerable()
                .Select(l => l.LevelSetuppable.Unload()).ToArray());
            
            if (includeEverPresent)
                await UniTask.WhenAll(_everPresentLevelSetuppables.AsValueEnumerable()
                    .Select(l => l.Unload()).ToArray());

            for (int i = 0; i < _tempSceneBoundBuffer.Count; i++)
                _sceneBound.Remove(_tempSceneBoundBuffer[i]);
            
            _tempSceneBoundBuffer.Clear();
        }
    }
}
