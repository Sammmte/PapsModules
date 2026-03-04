using Cysharp.Threading.Tasks;
using Paps.Logging;
using Paps.Optionals;
using Paps.SceneLoading;
using Paps.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.Levels
{
    [DefaultExecutionOrder(-10000)]
    public class LevelManager : MonoBehaviour
    {
        [Serializable]
        public struct LoadLevelOptions
        {
            [SerializeField] public bool DoGCCollect;
            [SerializeField] public bool UnloadUnusedAssets;
        }

        public enum Stage
        {
            NotInitialized,
            Loading,
            LevelLoaded,
            Unloding
        }

        public static LevelManager Instance { get; private set; }

        [SerializeField] private int _everPresentObjectsCapacity = 50;
        [SerializeField] private int _levelScenesCapacity = 10;
        [SerializeField] private int _rootGameObjectsCapacity = 1000;
        [SerializeField] private int _allBoundsCapacity = 10000;
        
        public Level CurrentLevel { get; private set; }
        public Stage CurrentStage { get; private set; }
        private List<ILevelBound> _everPresentBounds;
        private List<ILevelBound> _activeNonEverPresentBounds;
        private FastRemoveList<ILevelReadinessContributor> _readinessContributors;
        private List<Scene> _levelScenes;
        private List<GameObject> _rootGameObjectsList;
        private List<ILevelBound> _tempGetComponentList;
        private List<ILevelBound> _loadedPendingList;
        private HashSet<ILevelBound> _loadedDoneList;
        private List<ILevelBound> _kickstartPendingList;
        private HashSet<ILevelBound> _kickstartDoneList;
        private List<ILevelBound> _unloadPendingList;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _everPresentBounds = new List<ILevelBound>(_everPresentObjectsCapacity);
            _activeNonEverPresentBounds = new List<ILevelBound>(_allBoundsCapacity);
            _readinessContributors = new FastRemoveList<ILevelReadinessContributor>(_allBoundsCapacity);
            _levelScenes = new List<Scene>(_levelScenesCapacity);
            _rootGameObjectsList = new List<GameObject>(_rootGameObjectsCapacity);
            _tempGetComponentList = new List<ILevelBound>(_allBoundsCapacity);
            _loadedPendingList = new List<ILevelBound>(_allBoundsCapacity);
            _loadedDoneList = new HashSet<ILevelBound>(_allBoundsCapacity);
            _kickstartPendingList = new List<ILevelBound>(_allBoundsCapacity);
            _kickstartDoneList = new HashSet<ILevelBound>(_allBoundsCapacity);
            _unloadPendingList = new List<ILevelBound>(_allBoundsCapacity);
        }

        public async UniTask LoadInitialLevel(Level level, Optional<LoadLevelOptions> loadLevelOptions = default)
        {
            var loadedScenes = SceneLoader.GetLoadedScenes();

            await SceneLoader.LoadNewSceneAndWaitOneFrame("LevelSetup_EmptyScene");
            await SceneLoader.UnloadAsync(loadedScenes);

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions.Value);
                
            await Load(level);
        }

        public async UniTask LoadLevel(Level level, Func<UniTask> onUnload = null, Optional<LoadLevelOptions> loadLevelOptions = default)
        {
            CurrentStage = Stage.Unloding;

            await Unload();

            if (onUnload != null)
                await onUnload();

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions);

            await Load(level);
        }

        private async UniTask Load(Level level)
        {
            CurrentStage = Stage.Loading;

            await SceneLoader.LoadAsync(level.InitialScenesGroup, LoadSceneMode.Additive);

            CurrentLevel = level;
            _levelScenes.AddRange(level.InitialScenesGroup);

            SceneLoader.SetActiveScene(level.ActiveScene);

            await UniTask.NextFrame();

            LoadLevelBounds();

            await WaitForLevelToBeReady();

            CurrentStage = Stage.LevelLoaded;

            KickstartLevelBounds();
        }

        private async UniTask Unload(Func<UniTask> onUnload = null)
        {
            var sceneGroup = _levelScenes.ToArray();

            UnloadLevelBounds();

            await SceneLoader.UnloadAsync(sceneGroup);

            _levelScenes.Clear();

            if (onUnload != null)
                await onUnload();
        }

        public void RegisterEverPresent(ILevelBound levelSetuppable)
        {
            if(CurrentStage != Stage.NotInitialized)
            {
                throw new IllegalOperationOnLevelStageException($"Cannot register ever present after {Stage.NotInitialized} stage");
            }

            if(_everPresentBounds.Contains(levelSetuppable))
                return;
            
            _everPresentBounds.Add(levelSetuppable);
        }

        public void RegisterReadinessContributor(ILevelReadinessContributor contributor)
        {
            if(CurrentStage != Stage.Loading)
            {
                this.LogWarning($"Ignored add of LevelReadinessContributor because level manager is not on {Stage.Loading} stage. Current stage: {CurrentStage}");
                return;
            }

            _readinessContributors.Add(contributor);
        }

        private void LoadLevelBounds()
        {
            _kickstartPendingList.AddRange(_everPresentBounds);
            _loadedPendingList.AddRange(_everPresentBounds);

            GatherLevelBounds(CurrentLevel.InitialScenesGroup);

            for(int i = 0; i < _loadedPendingList.Count; i++)
            {
                var bound = _loadedPendingList[i];

                _loadedDoneList.Add(bound);
                bound.Loaded();
            }

            _loadedPendingList.Clear();
        }

        private void GatherLevelBounds(Scene[] sceneGroup)
        {
            for (int i = 0; i < sceneGroup.Length; i++)
            {
                var scene = sceneGroup[i];
                scene.GetRootGameObjects(_rootGameObjectsList);

                for (int j = 0; j < _rootGameObjectsList.Count; j++)
                {
                    _rootGameObjectsList[j].GetComponentsInChildren(true, _tempGetComponentList);

                    _activeNonEverPresentBounds.AddRange(_tempGetComponentList);
                    
                    _loadedPendingList.AddRange(_tempGetComponentList);
                    _loadedPendingList.RemoveAll(b => _loadedDoneList.Contains(b));

                    _kickstartPendingList.AddRange(_tempGetComponentList);
                    
                    _tempGetComponentList.Clear();
                }
                
                _rootGameObjectsList.Clear();
            }
        }

        private async UniTask WaitForLevelToBeReady()
        {
            while(_readinessContributors.Count > 0)
            {
                foreach(var contributor in _readinessContributors)
                {
                    if(contributor.IsReady)
                        _readinessContributors.Remove(contributor);
                }

                await UniTask.NextFrame();
            }
        }

        private void KickstartLevelBounds()
        {
            foreach(var bound in _kickstartPendingList)
            {
                _kickstartDoneList.Add(bound);
                bound.Kickstart();
            }

            _kickstartPendingList.Clear();
        }

        private void UnloadLevelBounds()
        {
            _unloadPendingList.AddRange(_activeNonEverPresentBounds);
            _unloadPendingList.AddRange(_everPresentBounds);

            for(int i = 0; i < _unloadPendingList.Count; i++)
            {
                _unloadPendingList[i].Unload();
            }

            _unloadPendingList.Clear();
            _loadedPendingList.Clear();
            _loadedDoneList.Clear();
            _kickstartPendingList.Clear();
            _kickstartDoneList.Clear();
            _activeNonEverPresentBounds.Clear();
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

        public bool DidLoaded(ILevelBound levelBound) => _loadedDoneList.Contains(levelBound);
        public bool DidKickstart(ILevelBound levelBound) => _kickstartDoneList.Contains(levelBound);

        public T AddLevelBoundComponent<T>(GameObject gameObject) where T : Component, ILevelBound
        {
            if(CurrentStage == Stage.Loading)
            {
                var component = gameObject.AddComponent<T>();
                _activeNonEverPresentBounds.Add(component);
                _kickstartPendingList.Add(component);

                _loadedDoneList.Add(component);
                component.Loaded();

                return component;
            }
            else if(CurrentStage == Stage.LevelLoaded)
            {
                var component = gameObject.AddComponent<T>();
                _activeNonEverPresentBounds.Add(component);

                _loadedDoneList.Add(component);
                component.Loaded();

                _kickstartDoneList.Add(component);
                component.Kickstart();

                return component;
            }
            else if(CurrentStage == Stage.Unloding)
            {
                throw new IllegalOperationOnLevelStageException("It is not allowed to add a level bound component while unloading level");
            }

            throw new IllegalOperationOnLevelStageException($"It is not allowed to add a level bound component while on stage {CurrentStage}");
        }
    }
}
