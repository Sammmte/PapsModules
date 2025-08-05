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
        private static List<ILevelSetuppable> _tempLevelSetuppableBuffer = new List<ILevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);
        private static List<ILevelSetuppable> _tempGetComponentLevelSetuppableBuffer = new List<ILevelSetuppable>(DEFAULT_OBJECTS_CAPACITY);

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
                    _rootGameObjectsList[j].GetComponentsInChildren(true, _tempGetComponentLevelSetuppableBuffer);

                    for (int k = 0; k < _tempGetComponentLevelSetuppableBuffer.Count; k++)
                    {
                        _sceneBound.Add(new SceneBoundLevelSetuppable(_tempGetComponentLevelSetuppableBuffer[k], scene));
                    }
                    
                    _tempLevelSetuppableBuffer.AddRange(_tempGetComponentLevelSetuppableBuffer);
                    
                    _tempGetComponentLevelSetuppableBuffer.Clear();
                }
                
                _rootGameObjectsList.Clear();
            }
            
            if(includeEverPresent)
                _tempLevelSetuppableBuffer.InsertRange(0, _everPresentLevelSetuppables);

            await SetupAndKickstartFromTempBuffer();
        }

        private static async UniTask UnloadFrom(SceneGroup sceneGroup, bool includeEverPresent)
        {
            for (int i = 0; i < _sceneBound.Count; i++)
            {
                if (sceneGroup.Scenes.Contains(_sceneBound[i].Scene))
                {
                    _tempLevelSetuppableBuffer.Add(_sceneBound[i].LevelSetuppable);
                    
                    _sceneBound.RemoveAt(i);

                    i--;
                }
            }
            
            if(includeEverPresent)
                _tempLevelSetuppableBuffer.AddRange(_everPresentLevelSetuppables);

            await UnloadFromTempBuffer();
        }

        public static T Instantiate<T>(T original) where T : Component
        {
            var instance = GameObject.Instantiate(original);

            instance.gameObject.GetComponentsInChildren(true, _tempLevelSetuppableBuffer);

            for (int i = 0; i < _tempLevelSetuppableBuffer.Count; i++)
            {
                _sceneBound.Add(new SceneBoundLevelSetuppable(_tempLevelSetuppableBuffer[i], instance.gameObject.scene));
            }

            SetupAndKickstartFromTempBuffer();

            return instance;
        }

        public static T AddSetuppableComponent<T>(this GameObject gameObject) where T : Component, ILevelSetuppable
        {
            var levelSetuppable = gameObject.AddComponent<T>();
            
            _sceneBound.Add(new SceneBoundLevelSetuppable(levelSetuppable, gameObject.scene));

            SetupAndKickstart(levelSetuppable);

            return levelSetuppable;
        }

        private static async UniTask SetupAndKickstartFromTempBuffer()
        {
            var array = _tempLevelSetuppableBuffer.ToArray();
            
            _tempLevelSetuppableBuffer.Clear();

            await UniTask.WhenAll(array.ToArray(l => l.Setup()));

            for (int i = 0; i < array.Length; i++)
            {
                array[i].Kickstart();
            }
        }

        private static async UniTask UnloadFromTempBuffer()
        {
            var array = _tempLevelSetuppableBuffer.ToArray();
            
            _tempLevelSetuppableBuffer.Clear();

            await UniTask.WhenAll(array.ToArray(l => l.Unload()));
        }

        private static async UniTask SetupAndKickstart(ILevelSetuppable levelSetuppable)
        {
            await levelSetuppable.Setup();
            
            levelSetuppable.Kickstart();
        }
    }
}
