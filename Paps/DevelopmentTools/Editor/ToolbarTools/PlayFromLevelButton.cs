using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("PlayFromLevelButton", order: -999)]
    public class PlayFromLevelButton : Button
    {
        private static bool _willPlayFromEntry;

        [InitializeOnLoadMethod]
        private static void ListenToPlayStateChange()
        {
            EditorSceneManager.playModeStartScene = null;
            EditorApplication.playModeStateChanged += SetEntrySceneOnBeforePlay;
        }

        private static void SetEntrySceneOnBeforePlay(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode && _willPlayFromEntry)
            {
                var entryScenePath = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(entryScenePath);
            }
        }

        public void InitializeElement()
        {
            iconImage = Background.FromTexture2D(EditorGUIUtility.IconContent("Animation.Play").image as Texture2D);
            text = "Level";
            clicked += PlayFromLevel;
        }

        private void PlayFromLevel()
        {
            if(EditorLevelManager.CurrentLoadedLevel == null)
            {
                Debug.LogError("No level loaded");
                return;
            }

            _willPlayFromEntry = true;
            SetupGameInEditor.SetSetupMode(SetupGameInEditor.SetupMode.Level);
            EditorApplication.EnterPlaymode();
        }
    }
}
