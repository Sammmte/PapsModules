using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;

namespace Paps.LevelSetup.Editor
{
    [MainToolbarElement("PlayFromEntryButton")]
    public class PlayFromEntryButton : Button
    {
        [UnityMainToolbarElementAttribute("PlayFromEntryButton")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        private static bool _willPlayFromEntry;

        [InitializeOnLoadMethod]
        private static void ListenToPlayStateChange()
        {
            EditorSceneManager.playModeStartScene = null;
            EditorApplication.playModeStateChanged += SetEntrySceneOnBeforePlay;
        }

        private static void SetEntrySceneOnBeforePlay(PlayModeStateChange stateChange)
        {
            if(stateChange == PlayModeStateChange.ExitingEditMode && _willPlayFromEntry)
            {
                var entryScenePath = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(entryScenePath);
            }
        }

        public void InitializeElement()
        {
            iconImage = Background.FromTexture2D(EditorGUIUtility.IconContent("Animation.Play").image as Texture2D);
            text = "Entry";
            clicked += PlayFromEntry;
        }

        private void PlayFromEntry()
        {
            _willPlayFromEntry = true;
            SetupGameInEditor.SetSetupMode(SetupGameInEditor.SetupMode.None);
            EditorApplication.EnterPlaymode();
        }
    }
}
