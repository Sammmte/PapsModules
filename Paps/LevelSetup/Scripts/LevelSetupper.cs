using Cysharp.Threading.Tasks;
using Paps.SceneLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.LevelSetup
{
    public static class LevelSetupper
    {
        private static Level _currentLevel;
        private static Dictionary<Scene, List<ILevelSetuppable>> _sceneBoundLevelSetuppables = new Dictionary<Scene, List<ILevelSetuppable>>();
        private static HashSet<ILevelSetuppable> _everPresentLevelSetuppables = new HashSet<ILevelSetuppable>();
        private static List<Scene> _levelScenes = new List<Scene>();

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

            var levelSetuppables = GetAllExistingLevelSetuppablesFrom(scenes);

            await SetupAndKickstartLevelSetuppables(levelSetuppables);
        }

        public static async UniTask UnloadScenesFromLevel(SceneGroup scenes)
        {
            var levelSetuppables = GetAllExistingLevelSetuppablesFrom(scenes);

            await UniTask.WhenAll(levelSetuppables.Select(l => l.Unload()));

            UnregisterFromScenes(scenes);

            await SceneLoader.UnloadAsync(scenes);

            foreach(var sceneName in scenes.Scenes)
                _levelScenes.Remove(sceneName);
        }

        private static void UnregisterFromScenes(SceneGroup sceneGroup)
        {
            for (int i = 0; i < sceneGroup.Scenes.Length; i++)
                _sceneBoundLevelSetuppables.Remove(sceneGroup.Scenes[i]);
        }

        private static async UniTask Load(Level level)
        {
            await SceneLoader.LoadAsync(level.InitialScenesGroup, LoadSceneMode.Additive);

            _currentLevel = level;
            _levelScenes.AddRange(level.InitialScenesGroup.Scenes);

            SceneLoader.SetActiveScene(level.ActiveScene);

            await UniTask.NextFrame();

            var levelSetuppables = _sceneBoundLevelSetuppables.Values.SelectMany(l => l)
                .Concat(_everPresentLevelSetuppables);

            await SetupAndKickstartLevelSetuppables(levelSetuppables);
        }

        private static async UniTask SetupAndKickstartLevelSetuppables(IEnumerable<ILevelSetuppable> levelSetuppables)
        {
            await UniTask.WhenAll(levelSetuppables.Select(l => l.Setup()));

            foreach (var levelSetuppable in levelSetuppables)
                levelSetuppable.Kickstart();
        }

        private static async UniTask Unload(Func<UniTask> onUnload = null)
        {
            await UnloadLevelSetuppables(GetAllExistingLevelSetuppables());

            _sceneBoundLevelSetuppables.Clear();

            await SceneLoader.UnloadAsync(new SceneGroup(_levelScenes.ToArray()));

            _levelScenes.Clear();

            if (onUnload != null)
                await onUnload();
        }

        private static async UniTask UnloadLevelSetuppables(IEnumerable<ILevelSetuppable> levelSetuppables)
        {
            await UniTask.WhenAll(GetAllExistingLevelSetuppables().Select(l => l.Unload()));
        }

        private static IEnumerable<ILevelSetuppable> GetAllExistingLevelSetuppables()
        {
            var list = new List<ILevelSetuppable>();

            foreach (var levelSetuppables in _sceneBoundLevelSetuppables.Values)
            {
                for (int i = 0; i < levelSetuppables.Count; i++)
                {
                    var levelSetuppable = levelSetuppables[i];
                    if(Exists(levelSetuppable))
                        list.Add(levelSetuppable);
                }
            }

            foreach (var levelSetuppable in _everPresentLevelSetuppables)
            {
                if(Exists(levelSetuppable))
                    list.Add(levelSetuppable);
            }

            return list;
        }

        private static IEnumerable<ILevelSetuppable> GetAllExistingLevelSetuppablesFrom(SceneGroup sceneGroup)
        {
            var list = new List<ILevelSetuppable>();

            foreach (var sceneWithSetuppables in _sceneBoundLevelSetuppables)
            {
                if (sceneGroup.Scenes.Contains(sceneWithSetuppables.Key))
                {
                    var levelSetuppables = sceneWithSetuppables.Value;
                    
                    for (int i = 0; i < levelSetuppables.Count; i++)
                    {
                        var levelSetuppable = levelSetuppables[i];
                        if(Exists(levelSetuppable))
                            list.Add(levelSetuppable);
                    }
                }
            }

            return list;
        }

        private static bool Exists(ILevelSetuppable levelSetuppable)
        {
            if (levelSetuppable is UnityEngine.Object obj) // needed to check if unity object was destroyed
                return obj != null;

            return levelSetuppable != null;
        }

        public static void RegisterSceneBound(Scene scene, ILevelSetuppable[] levelSetuppables)
        {
            if(!_sceneBoundLevelSetuppables.ContainsKey(scene))
                _sceneBoundLevelSetuppables.Add(scene, new List<ILevelSetuppable>());

            _sceneBoundLevelSetuppables[scene].AddRange(levelSetuppables);
        }

        public static void RegisterEverPresent(ILevelSetuppable levelSetuppable)
        {
            _everPresentLevelSetuppables.Add(levelSetuppable);
        }
    }
}
