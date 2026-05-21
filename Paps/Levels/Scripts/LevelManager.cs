using Cysharp.Threading.Tasks;
using Paps.Logging;
using Paps.Optionals;
using Paps.SceneLoading;
using Paps.UnityExtensions;
using SaintsField.Playa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Paps.SceneLoading.Scene;

[assembly: InternalsVisibleTo("Paps.Levels.Editor")]

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

        private class LevelBoundWrapper
        {
            public ILevelBound LevelBound { get; }

            public bool IsLoading { get; private set; }
            public bool DidLoad { get; private set; }
            public bool IsSetupping { get; private set; }
            public bool DidSetup { get; private set; }
            public bool IsKickstarting { get; private set; }
            public bool DidKickstart { get; private set; }
            public bool IsUnloading { get; private set; }
            public bool DidUnload { get; private set; }

            private CancellationTokenSource _cancellationTokenSource;

            private CancellationToken CancellationToken => _cancellationTokenSource.Token;

            public LevelBoundWrapper(ILevelBound levelBound, CancellationTokenSource unloadLevelTokenSource)
            {
                LevelBound = levelBound;
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(unloadLevelTokenSource.Token);
            }

            public void Construct() => LevelBound.Construct();

            public async UniTask Load()
            {
                if(IsLoading || DidLoad)
                    return;

                IsLoading = true;
                await LevelBound.Load(CancellationToken);
                DidLoad = true;
            }

            public async UniTask Setup()
            {
                if(IsSetupping || DidSetup)
                    return;

                await Load();
                
                if(CancellationToken.IsCancellationRequested)
                    return;

                IsSetupping = true;
                await LevelBound.Setup(CancellationToken);
                DidSetup = true;
            }

            public async UniTask Kickstart()
            {
                if(IsKickstarting || DidKickstart)
                    return;

                await Setup();

                if(CancellationToken.IsCancellationRequested)
                    return;

                IsKickstarting = true;
                LevelBound.Kickstart();
                DidKickstart = true;
            }

            public void Unload()
            {
                if(IsUnloading || DidUnload)
                    return;

                IsUnloading = true;
                
                _cancellationTokenSource.Cancel();

                LevelBound.Unload();
                DidUnload = true;
            }
        }

        public enum Stage
        {
            NotInitialized,
            Loading,
            LevelLoaded,
            Unloding
        }

        public enum LoadingSubStage
        {
            None,
            Construct,
            Load,
            Setup
        }

        public static LevelManager Instance { get; private set; }

        [SerializeField] private int _levelScenesCapacity = 10;
        [SerializeField] private int _rootGameObjectsCapacity = 1000;
        [SerializeField] private int _allBoundsCapacity = 10000;
        [field: SerializeField] public LevelList LevelList;
        
        [ShowInInspector] public Level CurrentLevel { get; private set; }
        [ShowInInspector] public Stage CurrentStage { get; private set; }
        [ShowInInspector] public LoadingSubStage CurrentLoadingSubStage { get; private set; }
        [ShowInInspector] public int LevelBoundsCount => _activeBounds.Count;
        
        private List<LevelBoundWrapper> _activeBounds;
        private Dictionary<ILevelBound, LevelBoundWrapper> _activeBoundsByLevelBound;
        private List<Scene> _levelScenes;
        private List<GameObject> _rootGameObjectsList;
        private List<ILevelBound> _tempGetComponentList;
        private List<ILevelSetup> _currentLevelSetups;

        private List<LevelBoundWrapper> _initializationExecutionList;
        private List<LevelBoundWrapper> _tempExecutionList;

        private CancellationTokenSource _unloadLevelOrQuitTokenSource;

        private CancellationToken LevelUnloadOrQuitCancellationToken => _unloadLevelOrQuitTokenSource.Token;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _activeBounds = new List<LevelBoundWrapper>(_allBoundsCapacity);
            _activeBoundsByLevelBound = new Dictionary<ILevelBound, LevelBoundWrapper>(_allBoundsCapacity);
            _levelScenes = new List<Scene>(_levelScenesCapacity);
            _rootGameObjectsList = new List<GameObject>(_rootGameObjectsCapacity);
            _tempGetComponentList = new List<ILevelBound>(_allBoundsCapacity);
            _currentLevelSetups = new List<ILevelSetup>();

            _initializationExecutionList = new List<LevelBoundWrapper>(_allBoundsCapacity);
            _tempExecutionList = new List<LevelBoundWrapper>(_allBoundsCapacity);
        }

        private async UniTask LoadInitialLevel(Level level, Func<UniTask> onUnload = null, Optional<LoadLevelOptions> loadLevelOptions = default,
            IEnumerable<ILevelSetup> extraLevelSetups = null)
        {
            this.Log($"Loading initial level <color=green>{level.Id}</color>");

            var loadedScenes = SceneLoader.GetLoadedScenes();

            await SceneLoader.LoadNewSceneAndWaitOneFrame("LevelSetup_EmptyScene");
            await SceneLoader.UnloadAsync(loadedScenes);

            if (onUnload != null)
                await onUnload();

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions.Value);
                
            await Load(level, extraLevelSetups);
        }

        public async UniTask LoadLevel(Level level, Func<UniTask> onUnload = null, Optional<LoadLevelOptions> loadLevelOptions = default,
            IEnumerable<ILevelSetup> extraLevelSetups = null)
        {
            ThrowIfOnAnyStage(stackalloc Stage[] { Stage.Loading, Stage.Unloding }, nameof(LoadLevel));

            if(CurrentLevel == null)
            {
                await LoadInitialLevel(level, onUnload, loadLevelOptions, extraLevelSetups);
            }
            else
            {
                await LoadNonInitialLevel(level, onUnload, loadLevelOptions, extraLevelSetups);
            }
        }

        private async UniTask LoadNonInitialLevel(Level level, Func<UniTask> onUnload = null, Optional<LoadLevelOptions> loadLevelOptions = default,
            IEnumerable<ILevelSetup> extraLevelSetups = null)
        {
            this.Log($"Unloading level <color=red>{CurrentLevel.Id}</color>");

            CurrentStage = Stage.Unloding;

            await Unload();

            if (onUnload != null)
                await onUnload();

            if(loadLevelOptions.HasValue)
                await ApplyGCCollectOrAssetUnload(loadLevelOptions);

            this.Log($"Loading level <color=green>{level.Id}</color>");

            await Load(level, extraLevelSetups);
        }

        private async UniTask Load(Level level, IEnumerable<ILevelSetup> extraLevelSetups)
        {
            CurrentStage = Stage.Loading;

            _unloadLevelOrQuitTokenSource = CreateCancellationTokenSource();

            await SceneLoader.LoadAsync(level.InitialScenesGroup, LoadSceneMode.Additive);

            CurrentLevel = level;
            _levelScenes.AddRange(level.InitialScenesGroup);

            SceneLoader.SetActiveScene(level.ActiveScene);

            await UniTask.NextFrame();

            CurrentLoadingSubStage = LoadingSubStage.Construct;

            GatherLevelBounds(CurrentLevel.InitialScenesGroup);

            CurrentLoadingSubStage = LoadingSubStage.Load;

            await LoadLevelSetups(extraLevelSetups);

            await LoadLevelBounds();

            CurrentLoadingSubStage = LoadingSubStage.Setup;

            await SetupLevelSetups();

            await SetupLevelBounds();

            CurrentStage = Stage.LevelLoaded;
            CurrentLoadingSubStage = LoadingSubStage.None;

            KickstartLevelSetups();

            KickstartLevelBounds();
        }

        private async UniTask Unload(Func<UniTask> onUnload = null)
        {
            var sceneGroup = _levelScenes.ToArray();

            _unloadLevelOrQuitTokenSource.Cancel();
            _unloadLevelOrQuitTokenSource = null;

            UnloadLevelBounds();

            UnloadLevelSetups();

            await SceneLoader.UnloadAsync(sceneGroup);

            _levelScenes.Clear();

            if (onUnload != null)
                await onUnload();
        }

        private async UniTask LoadLevelSetups(IEnumerable<ILevelSetup> extraLevelSetups)
        {
            if(extraLevelSetups != null)
            {
                _currentLevelSetups.AddRange(extraLevelSetups);
            }

            _currentLevelSetups.AddRange(CurrentLevel.LevelSetups);

            await UniTask.WhenAll(_currentLevelSetups.Select(s => s.Load(LevelUnloadOrQuitCancellationToken)));
        }

        private async UniTask LoadLevelBounds()
        {
            for(int i = 0; i < _activeBounds.Count; i++)
            {
                var levelBound = _activeBounds[i];

                _initializationExecutionList.Add(levelBound);
                _tempExecutionList.Add(levelBound);
            }

            for(int i = 0; i < _tempExecutionList.Count; i++)
            {
                var levelBound = _tempExecutionList[i];

                AwaitTaskAndRemove(levelBound, levelBound.Load());
            }

            _tempExecutionList.Clear();

            await WaitForInitializationList();
        }

        private async UniTask AwaitTaskAndRemove(LevelBoundWrapper levelBound, UniTask task)
        {
            await task;

            _initializationExecutionList.Remove(levelBound);
        }

        private async UniTask WaitForInitializationList()
        {
            while(_initializationExecutionList.Count > 0)
            {
                await UniTask.NextFrame();
            }
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

                    foreach(var levelBound in _tempGetComponentList)
                    {
                        var wrapper = CreateLevelBoundWrapper(levelBound);
                    }
                    
                    _tempGetComponentList.Clear();
                }
                
                _rootGameObjectsList.Clear();
            }
        }

        private async UniTask SetupLevelSetups()
        {
            await UniTask.WhenAll(_currentLevelSetups.Select(s => s.Setup(LevelUnloadOrQuitCancellationToken)));
        }

        private async UniTask SetupLevelBounds()
        {
            for(int i = 0; i < _activeBounds.Count; i++)
            {
                var levelBound = _activeBounds[i];

                _initializationExecutionList.Add(levelBound);
                _tempExecutionList.Add(levelBound);
            }

            for(int i = 0; i < _tempExecutionList.Count; i++)
            {
                var levelBound = _tempExecutionList[i];

                AwaitTaskAndRemove(levelBound, levelBound.Setup());
            }

            _tempExecutionList.Clear();

            await WaitForInitializationList();
        }

        private void KickstartLevelSetups()
        {
            for(int i = 0; i < _currentLevelSetups.Count; i++)
            {
                _currentLevelSetups[i].Kickstart();

                if(LevelUnloadOrQuitCancellationToken.IsCancellationRequested)
                    return;
            }
        }

        private void KickstartLevelBounds()
        {
            for(int i = 0; i < _activeBounds.Count; i++)
            {
                var levelBound = _activeBounds[i];

                _tempExecutionList.Add(levelBound);
            }

            for(int i = 0; i < _tempExecutionList.Count; i++)
            {
                var levelBound = _tempExecutionList[i];

                levelBound.Kickstart().Forget();

                if(LevelUnloadOrQuitCancellationToken.IsCancellationRequested)
                {
                    _tempExecutionList.Clear();
                    return;
                }
            }

            _tempExecutionList.Clear();
        }

        private void UnloadLevelSetups()
        {
            for(int i = 0; i < _currentLevelSetups.Count; i++)
            {
                _currentLevelSetups[i].Unload();
            }

            _currentLevelSetups.Clear();
        }

        private void UnloadLevelBounds()
        {
            for(int i = 0; i < _activeBounds.Count; i++)
            {
                _activeBounds[i].Unload();
            }

            _initializationExecutionList.Clear();
            _activeBoundsByLevelBound.Clear();
            _activeBounds.Clear();
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

        public T AddLevelBoundComponent<T>(GameObject gameObject) where T : Component, ILevelBound
        {
            if(CurrentStage == Stage.Unloding)
            {
                throw new IllegalOperationOnLevelStageException("It is not allowed to add a level bound component while unloading level");
            }

            var levelBound = gameObject.AddComponent<T>();

            var wrapper = CreateLevelBoundWrapper(levelBound);

            if(CurrentStage == Stage.Loading)
            {
                AddExternalLevelBoundWhileOnLoading(wrapper);

                return levelBound;
            }
            else if(CurrentStage == Stage.LevelLoaded)
            {
                AddExternalLevelBoundAfterLevelLoaded(wrapper);

                return levelBound;
            }

            throw new IllegalOperationOnLevelStageException($"It is not allowed to add a level bound component while on stage {CurrentStage}");
        }

        private void AddExternalLevelBoundWhileOnLoading(LevelBoundWrapper wrapper)
        {
            _initializationExecutionList.Add(wrapper);

            switch(CurrentLoadingSubStage)
            {
                case LoadingSubStage.Load:
                    AwaitTaskAndRemove(wrapper, wrapper.Load());
                    return;

                case LoadingSubStage.Setup:
                    AwaitTaskAndRemove(wrapper, wrapper.Setup());
                    return;
            }
        }

        private async UniTask AddExternalLevelBoundAfterLevelLoaded(LevelBoundWrapper wrapper)
        {
            await wrapper.Kickstart();
        }

        private LevelBoundWrapper CreateLevelBoundWrapper(ILevelBound levelBound)
        {
            var wrapper = new LevelBoundWrapper(levelBound, _unloadLevelOrQuitTokenSource);

            _activeBounds.Add(wrapper);
            _activeBoundsByLevelBound.Add(levelBound, wrapper);

            wrapper.Construct();

            return wrapper;
        }

        private CancellationTokenSource CreateCancellationTokenSource()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
        }

        private void ThrowIfOnStage(Stage stage, string operationName)
        {
            if(CurrentStage == stage)
            {
                throw new IllegalOperationOnLevelStageException($"It is not allowed to execute operation {operationName} while on stage {CurrentStage}");
            }
        }

        private void ThrowIfOnAnyStage(ReadOnlySpan<Stage> stages, string operationName)
        {
            for(int i = 0; i < stages.Length; i++)
            {
                if(CurrentStage == stages[i])
                {
                    throw new IllegalOperationOnLevelStageException($"It is not allowed to execute operation {operationName} while on stage {CurrentStage}");
                }
            }
        }

        private void ThrowIfNotOnStage(Stage stage, string operationName)
        {
            if(CurrentStage == stage)
            {
                throw new IllegalOperationOnLevelStageException($"It is not allowed to execute operation {operationName} while on stage {CurrentStage}");
            }
        }

        public bool IsLoading(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.IsLoading)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].IsLoading;
        }
        public bool DidLoad(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.DidLoad)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].DidLoad;
        }
        public bool IsSetupping(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.IsSetupping)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].IsSetupping;
        }
        public bool DidSetup(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.DidSetup)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].DidSetup;
        }
        public bool IsKickstarting(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.IsKickstarting)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].IsKickstarting;
        }
        public bool DidKickstart(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.DidKickstart)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].DidKickstart;
        }
        public bool IsUnloading(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.IsUnloading)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].IsUnloading;
        }
        public bool DidUnload(ILevelBound levelBound)
        {
            if(levelBound == null)
                throw new ArgumentNullException(nameof(levelBound));

            if(!_activeBoundsByLevelBound.ContainsKey(levelBound))
            {
                this.LogWarning($"Tried to query {nameof(LevelBoundWrapper.DidUnload)} state but level bound {levelBound.GetUnityName()} is not yet/anymore tracked by LevelManager");
                return false;
            }

            return _activeBoundsByLevelBound[levelBound].DidUnload;
        }
    }
}
