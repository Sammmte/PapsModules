using Cysharp.Threading.Tasks;
using Paps.Optionals;
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
    [DefaultExecutionOrder(-10000)]
    public class LevelSetupper : MonoBehaviour
    {
        [Serializable]
        public struct LoadLevelOptions
        {
            [SerializeField] public bool DoGCCollect;
            [SerializeField] public bool UnloadUnusedAssets;
        }

        public static LevelSetupper Instance { get; private set; }

        [SerializeField] private int _everPresentObjectsCapacity = 50;
        [SerializeField] private int _levelScenesCapacity = 10;
        [SerializeField] private int _rootGameObjectsCapacity = 1000;
        [SerializeField] private int _sceneBoundObjectsCapacity = 1000;
        
        private Level _currentLevel;
        private List<ILevelSetuppable> _everPresentLevelSetuppables;
        private List<Scene> _levelScenes;
        private List<GameObject> _rootGameObjectsList;
        private List<SceneBoundLevelSetuppable> _sceneBound;
        private List<ILevelSetuppable> _tempLevelSetuppableBuffer;
        private List<ILevelSetuppable> _tempGetComponentLevelSetuppableBuffer;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _everPresentLevelSetuppables = new List<ILevelSetuppable>(_everPresentObjectsCapacity);
            _levelScenes = new List<Scene>(_levelScenesCapacity);
            _rootGameObjectsList = new List<GameObject>(_rootGameObjectsCapacity);
            _sceneBound = new List<SceneBoundLevelSetuppable>(_sceneBoundObjectsCapacity);
            _tempLevelSetuppableBuffer = new List<ILevelSetuppable>(_sceneBoundObjectsCapacity);
            _tempGetComponentLevelSetuppableBuffer = new List<ILevelSetuppable>(_sceneBoundObjectsCapacity);
        }

        public async UniTask LoadAndSetupInitialLevel(Level level, Optional<LoadLevelOptions> loadLevelOptions = default)
        {
            var loadedScenes = SceneLoader.GetLoadedScenes();

            await SceneLoader.LoadNewSceneAndWaitOneFrame("LevelSetup_EmptyScene");
            await SceneLoader.UnloadAsync(loadedScenes);

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions.Value);
                
            await Load(level);
        }

        public async UniTask LoadAndSetupLevel(Level level, Func<UniTask> onUnload = null, Optional<LoadLevelOptions> loadLevelOptions = default)
        {
            await Unload();

            if (onUnload != null)
                await onUnload();

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions);

            await Load(level);
        }

        private async UniTask Load(Level level)
        {
            await SceneLoader.LoadAsync(level.InitialScenesGroup, LoadSceneMode.Additive);

            _currentLevel = level;
            _levelScenes.AddRange(level.InitialScenesGroup);

            SceneLoader.SetActiveScene(level.ActiveScene);

            await UniTask.NextFrame();

            await SetupAndKickstartFrom(level.InitialScenesGroup, true);
        }

        private async UniTask Unload(Func<UniTask> onUnload = null)
        {
            var sceneGroup = _levelScenes.ToArray();

            await UnloadFrom(sceneGroup, true);

            await SceneLoader.UnloadAsync(sceneGroup);

            _levelScenes.Clear();

            if (onUnload != null)
                await onUnload();
        }

        public void RegisterEverPresent(ILevelSetuppable levelSetuppable)
        {
            if(_everPresentLevelSetuppables.Contains(levelSetuppable))
                return;
            
            _everPresentLevelSetuppables.Add(levelSetuppable);
        }

        private async UniTask SetupAndKickstartFrom(Scene[] sceneGroup, bool includeEverPresent)
        {
            for (int i = 0; i < sceneGroup.Length; i++)
            {
                var scene = sceneGroup[i];
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

        private async UniTask UnloadFrom(Scene[] sceneGroup, bool includeEverPresent)
        {
            for (int i = 0; i < _sceneBound.Count; i++)
            {
                if (sceneGroup.Contains(_sceneBound[i].Scene))
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

        private async UniTask SetupAndKickstartFromTempBuffer()
        {
            var array = _tempLevelSetuppableBuffer.ToArray();
            
            _tempLevelSetuppableBuffer.Clear();
            
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Created();
            }

            await UniTask.WhenAll(array.ToArray(l => l.Setup()));

            for (int i = 0; i < array.Length; i++)
            {
                array[i].Kickstart();
            }
        }

        private async UniTask UnloadFromTempBuffer()
        {
            var array = _tempLevelSetuppableBuffer.ToArray();
            
            _tempLevelSetuppableBuffer.Clear();

            await UniTask.WhenAll(array.ToArray(l => l.Unload()));
        }

        public T AddSetuppableComponent<T>(GameObject gameObject) where T : Component, ILevelSetuppable
        {
            var levelSetuppable = gameObject.AddComponent<T>();
            
            _sceneBound.Add(new SceneBoundLevelSetuppable(levelSetuppable, gameObject.scene));

            SetupAndKickstart(levelSetuppable);

            return levelSetuppable;
        }

        private async UniTask SetupAndKickstart(ILevelSetuppable levelSetuppable)
        {
            levelSetuppable.Created();
            
            await levelSetuppable.Setup();
            
            levelSetuppable.Kickstart();
        }

        private async UniTask ApplyGCCollectOrAssetUnload(LoadLevelOptions loadLevelOptions)
        {
            if(loadLevelOptions.UnloadUnusedAssets)
            {
                await Resources.UnloadUnusedAssets();
            }

            if(loadLevelOptions.DoGCCollect)
            {
                GC.Collect();
            }
        }
    }
}
