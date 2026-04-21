using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.SceneLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Paps.EditorGameSetup
{
    public static class EditorGameSetupManager
    {
        private const string ENTRY_SCENE_NAME = "Entry";
        private const string SETUP_SCENE_NAME = "Setup";
        private const string SAVE_SCOPE = "editor-game-setup";
        private const string SAVE_STATE_KEY = "editor-game-setup-state";

        private static EditorGameSetupSettings _settings;
        private static EditorGameSetupState _currentState;

        [InitializeOnLoadMethod]
        private static void ListenForPlayModeStateChange()
        {
            EditorSceneManager.playModeStartScene = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SetupOnPlayMode()
        {
            _currentState = GetState();

            if (_currentState.SetupMode == EditorGameSetupMode.Custom)
            {
                AwaitSetupAndCallCustomSetuppers().Forget();
            }
        }

        private static async UniTask AwaitSetupAndCallCustomSetuppers()
        {
            await GameSetupManager.Instance.Setup();

            var settings = GetSettings();
            var context = new EditorGameSetupContext(GetFullScenesFromNames(_currentState.LoadedScenes));

            foreach (var setupper in settings.OrderedSetuppers)
            {
                await setupper.Setup(context);
            }
        }

        public static void EnterPlayMode(EditorGameSetupParameters parameters)
        {
            SaveState(parameters);

            if(parameters.SetupMode == EditorGameSetupMode.Custom)
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSetupScenePath());
            }
            else if(parameters.SetupMode == EditorGameSetupMode.Entry)
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GetEntryScenePath());
            }

            EditorApplication.EnterPlaymode();
        }

        private static void SaveState(EditorGameSetupParameters parameters)
        {
            var state = new EditorGameSetupState()
            {
                SetupMode = parameters.SetupMode,
                LoadedScenes = GetLoadedScenes()
            };

            UnityPrefs.UnityPrefs.GetPref(UnityPrefs.UnityPrefType.UserProjectPrefs, SAVE_SCOPE).Set(SAVE_STATE_KEY, state);
        }

        private static EditorGameSetupState GetState()
        {
            return UnityPrefs.UnityPrefs.GetPref(UnityPrefs.UnityPrefType.UserProjectPrefs, SAVE_SCOPE).Get<EditorGameSetupState>(SAVE_STATE_KEY);
        }

        private static string GetSetupScenePath()
        {
            return EditorBuildSettings.scenes.First(s => Path.GetFileNameWithoutExtension(s.path) == SETUP_SCENE_NAME).path;
        }

        private static string GetEntryScenePath()
        {
            return EditorBuildSettings.scenes.First(s => Path.GetFileNameWithoutExtension(s.path) == ENTRY_SCENE_NAME).path;
        }

        private static Scene[] GetFullScenesFromNames(string[] sceneNames)
        {
            var list = new List<Scene>(sceneNames.Length);

            for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                if (sceneNames.Contains(sceneName))
                {
                    list.Add(new Scene(scene.path, i));
                }
            }

            return list.ToArray();
        }

        private static string[] GetLoadedScenes()
        {
            var loadedScenes = new string[EditorSceneManager.sceneCount];

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var unityScene = EditorSceneManager.GetSceneAt(i);
                loadedScenes[i] = unityScene.name;
            }

            return loadedScenes;
        }

        private static EditorGameSetupSettings GetSettings()
        {
            if(_settings == null)
            {
                var guid = AssetDatabase.FindAssetGUIDs($"t:{nameof(EditorGameSetupSettings)}").First();

                _settings = AssetDatabase.LoadAssetByGUID<EditorGameSetupSettings>(guid);
            }

            return _settings;
        }
    }

    internal struct EditorGameSetupState
    {
        public EditorGameSetupMode SetupMode;
        public string[] LoadedScenes;
    }
}
