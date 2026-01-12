using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.LevelSetup.Editor
{
    public static class EditorLevelManager
    {
        private static Level _lastLoadedLevel;

        public static Level CurrentLoadedLevel { get; private set; }

        public static event Action<Level> OnLevelChanged;

        private static Dictionary<string, Level> _levels;

        private static bool _isInRefreshCooldown;
        private static float _refreshCooldown = 0.2f;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += RefreshOnPlayModeStateChange;

            EditorSceneManager.sceneOpened += (_, __) => RefreshCurrentLoadedLevel();
            EditorSceneManager.sceneLoaded += (_, __) => RefreshCurrentLoadedLevel();
            EditorSceneManager.sceneUnloaded += _ => RefreshCurrentLoadedLevel();
            EditorSceneManager.sceneClosed += _ => RefreshCurrentLoadedLevel();
            EditorSceneManager.activeSceneChanged += (_, __) => RefreshCurrentLoadedLevel();
            EditorSceneManager.activeSceneChangedInEditMode += (_, __) => RefreshCurrentLoadedLevel();

            EditorApplication.projectChanged += RefreshLoadedLevels;

            EditorApplication.update += Update;

            RefreshLoadedLevels();
        }

        private static void Update()
        {
            if(CurrentLoadedLevel != _lastLoadedLevel)
                OnLevelChanged?.Invoke(CurrentLoadedLevel);

            _lastLoadedLevel = CurrentLoadedLevel;
        }

        private static void RefreshOnPlayModeStateChange(PlayModeStateChange stateChange)
        {
            if(stateChange == PlayModeStateChange.EnteredEditMode)
                RefreshCurrentLoadedLevel();
        }

        public static Level[] GetLevels()
        {
            return _levels.Values.ToArray();
        }

        public static Level GetLevelById(string id)
        {
            return _levels.Values.First(l => l.Id == id);
        }

        private static void RefreshCurrentLoadedLevel()
        {
            if (_isInRefreshCooldown)
                return;

            _isInRefreshCooldown = true;
            CurrentLoadedLevel = GetLoadedLevel();

            UniTask.Delay(TimeSpan.FromSeconds(_refreshCooldown))
                .ContinueWith(() => _isInRefreshCooldown = false)
                .Forget();
        }

        private static void RefreshLoadedLevels()
        {
            _levels = LoadLevels();
            RefreshCurrentLoadedLevel();
        }

        public static void LoadLevel(Level level, bool allScenes = false)
        {
            var scenes = level.InitialScenesGroup;

            if(allScenes)
            {
                scenes = scenes.Concat(level.ExtraScenes).ToArray();
            }

            EditorSceneManager.OpenScene(scenes[0].GetSceneAssetPath(), OpenSceneMode.Single);

            for (int i = 1; i < scenes.Length; i++)
                EditorSceneManager.OpenScene(scenes[i].GetSceneAssetPath(), OpenSceneMode.Additive);

            CurrentLoadedLevel = level;
        }

        private static bool IsValidLevel(Level level)
        {
            return !string.IsNullOrEmpty(level.Id) && level.InitialScenesGroup.Length > 0;
        }

        private static Dictionary<string, Level> LoadLevels()
        {
            var levels = AssetDatabase.FindAssets($"t:{nameof(Level)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Level>)
                .Where(IsValidLevel)
                .ToDictionary(l => l.Id, l => l);

            return levels;
        }

        private static Level GetLoadedLevel()
        {
            var loadedScenes = GetLoadedScenes();

            foreach(var level in _levels.Values)
            {
                if(InitialLevelScenesAreLoaded(level, loadedScenes))
                    return level;
            }

            return null;
        }

        private static Scene[] GetLoadedScenes()
        {
            var loadedScenes = new List<Scene>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                loadedScenes.Add(scene);
            }

            return loadedScenes.ToArray();
        }

        private static bool InitialLevelScenesAreLoaded(Level level, Scene[] loadedScenes)
        {
            for(int i = 0; i < level.InitialScenesGroup.Length; i++)
            {
                if(!SceneIsLoaded(level.InitialScenesGroup[i], loadedScenes))
                    return false;
            }

            return true;
        }

        private static bool SceneIsLoaded(Scene scene, Scene[] loadedScenes)
        {
            for(int i = 0; i < loadedScenes.Length; i++)
            {
                if(loadedScenes[i].Equals(scene))
                    return true;
            }

            return false;
        }
    }
}
