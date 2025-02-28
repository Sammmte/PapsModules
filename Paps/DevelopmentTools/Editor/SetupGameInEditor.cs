using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using Paps.SceneLoading;
using Paps.StartupSetup;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.DevelopmentTools.Editor
{
    public static class SetupGameInEditor
    {
        public enum SetupMode
        {
            None,
            Dynamic,
            Level
        }

        private const string ENTRY_SCENE_NAME = "Entry", SETUP_SCENE_NAME = "Setup";
        private const string EDITOR_SCENE_STATE_KEY = "EDITOR_SCENE_STATE";
        private const string LOAD_LEVEL_KEY = "LOAD_LEVEL";

        private static SetupMode _setupMode = SetupMode.Dynamic;

        [InitializeOnLoadMethod]
        private static void ListenForPlayModeStateChange()
        {
            EditorSceneManager.playModeStartScene = null;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (ShouldSetupGame(state))
            {
                if(_setupMode == SetupMode.Level)
                    SaveEditorLevelState();
                else
                    SaveEditorSceneState();

                var setupScenePath = GetSetupScenePath();
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(setupScenePath);
            }
        }

        public static void SetSetupMode(SetupMode setupMode)
        {
            _setupMode = setupMode;
        }

        private static string GetSetupScenePath()
        {
            return EditorBuildSettings.scenes.First(s => Path.GetFileNameWithoutExtension(s.path).ToLower() == SETUP_SCENE_NAME.ToLower()).path;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstantiateGameSetupperAwaiter()
        {
            if (!CanExecuteAtRuntime())
                return;

            if(EditorPrefs.HasKey(EDITOR_SCENE_STATE_KEY))
                AwaitSetupAndOpenScene(GetEditorSceneState()).Forget();
            else
                AwaitSetupAndOpenLevel(GetEditorLevelState()).Forget();

            DeleteEditorSceneStateKey();
            DeleteEditorLevelStateKey();
        }

        private static async UniTaskVoid AwaitSetupAndOpenScene(EditorSceneState editorSceneState)
        {
            var setupper = Object.FindFirstObjectByType<StartupSetupper>();

            await setupper.Setup();

            var level = new Level("EditorSetupLevel", new SceneGroup(editorSceneState.OpenedScenes.Select<SceneDTO, Scene>(s => s).ToArray()), 
                editorSceneState.ActiveScene);

            await LevelSetupper.LoadAndSetupInitialLevel(level);
        }

        private static async UniTaskVoid AwaitSetupAndOpenLevel(EditorLevelState editorLevelState)
        {
            var setupper = Object.FindFirstObjectByType<StartupSetupper>();

            await setupper.Setup();

            var level = EditorLevelManager.GetLevelByName(editorLevelState.LevelName);

            await LevelSetupper.LoadAndSetupInitialLevel(level);
        }

        private static bool ShouldSetupGame(PlayModeStateChange state)
        {
            return state == PlayModeStateChange.ExitingEditMode && IsASceneOfOurGame() && IsOpeningOtherThanEntryOrSetup() && _setupMode != SetupMode.None;
        }

        private static bool IsASceneOfOurGame()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (EditorBuildSettings.scenes.Any(gameScene => gameScene.path == SceneManager.GetSceneAt(i).path))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsOpeningOtherThanEntryOrSetup()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.name != ENTRY_SCENE_NAME && scene.name != SETUP_SCENE_NAME)
                    return true;
            }

            return false;
        }

        private static EditorSceneState GetOpenedScenesInEditor()
        {
            var scenes = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes.Add(SceneManager.GetSceneAt(i));
            }

            return new EditorSceneState()
            {
                OpenedScenes = scenes.Select<Scene, SceneDTO>(s => s).ToArray(),
                ActiveScene = SceneManager.GetActiveScene()
            };
        }

        private static void SaveEditorSceneState() => EditorPrefs.SetString(EDITOR_SCENE_STATE_KEY, JsonSerialization.ToJson(GetOpenedScenesInEditor()));
        private static void SaveEditorLevelState() => EditorPrefs.SetString(LOAD_LEVEL_KEY,
            JsonSerialization.ToJson(new EditorLevelState() { LevelName = EditorLevelManager.CurrentLoadedLevel.Value.Name }));
 
        private static EditorSceneState GetEditorSceneState() => JsonSerialization.FromJson<EditorSceneState>(EditorPrefs.GetString(EDITOR_SCENE_STATE_KEY, "{}"));
        private static EditorLevelState GetEditorLevelState() => JsonSerialization.FromJson<EditorLevelState>(EditorPrefs.GetString(LOAD_LEVEL_KEY, "{}"));

        private static bool CanExecuteAtRuntime() => EditorPrefs.HasKey(EDITOR_SCENE_STATE_KEY) || EditorPrefs.HasKey(LOAD_LEVEL_KEY);

        private static void DeleteEditorSceneStateKey() => EditorPrefs.DeleteKey(EDITOR_SCENE_STATE_KEY);
        private static void DeleteEditorLevelStateKey() => EditorPrefs.DeleteKey(LOAD_LEVEL_KEY);
    }
}