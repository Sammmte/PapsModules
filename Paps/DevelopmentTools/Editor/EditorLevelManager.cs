using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.DevelopmentTools.Editor
{
    public static class EditorLevelManager
    {
        private static Level? _lastLoadedLevel;

        public static Level? CurrentLoadedLevel { get; private set; }

        public static event Action<Level?> OnLevelChanged;

        private static LevelWithExtraData[] _levels;

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
            if(!CurrentLoadedLevel.Equals(_lastLoadedLevel))
                OnLevelChanged?.Invoke(CurrentLoadedLevel);

            _lastLoadedLevel = CurrentLoadedLevel;
        }

        private static void RefreshOnPlayModeStateChange(PlayModeStateChange stateChange)
        {
            if(stateChange == PlayModeStateChange.EnteredEditMode)
                RefreshCurrentLoadedLevel();
        }

        public static LevelWithExtraData[] GetLevels()
        {
            return _levels;
        }

        public static Level GetLevelByName(string name)
        {
            return _levels.First(l => l.Name == name).Level;
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

        public static void LoadLevel(ScriptableLevel level, LevelEditorData levelEditorData)
        {
            var scenes = level.InitialScenesGroup.Scenes;

            if (levelEditorData != null)
                scenes = scenes.Concat(levelEditorData.ExtraScenes.Scenes).ToArray();

            EditorSceneManager.OpenScene(scenes[0].Path, OpenSceneMode.Single);

            for (int i = 1; i < scenes.Length; i++)
                EditorSceneManager.OpenScene(scenes[i].Path, OpenSceneMode.Additive);

            CurrentLoadedLevel = level;
        }

        private static LevelWithExtraData[] LoadLevels()
        {
            var scriptableLevels = AssetDatabase.FindAssets($"t:{nameof(ScriptableLevel)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableLevel>)
                .Where(l => l.InitialScenesGroup.Scenes.Length > 0);

            var levelEditorData = AssetDatabase.FindAssets($"t:{nameof(LevelEditorData)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelEditorData>);

            return scriptableLevels.Select(l => new LevelWithExtraData()
            {
                Name = l.Name,
                Level = l,
                LevelEditorData = levelEditorData.FirstOrDefault(data => data.Level == l)
            }
            ).ToArray();
        }

        private static Level? GetLoadedLevel()
        {
            var loadedScenes = GetLoadedScenes();

            foreach(var levelWithExtraData in _levels)
            {
                var currentLevel = levelWithExtraData.Level;

                if(currentLevel.InitialScenesGroup.Scenes.All(s => loadedScenes.Contains(s)))
                    return currentLevel;
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
    }
}
