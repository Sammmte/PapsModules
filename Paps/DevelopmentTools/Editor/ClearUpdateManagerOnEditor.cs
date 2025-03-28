using UnityEditor;
using Gilzoide.UpdateManager;

namespace Paps.DevelopmentTools.Editor
{
    public static class ClearUpdateManagerOnEditor
    {
        [InitializeOnLoadMethod]
        private static void ListenForPlayModeStateChange()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode || playModeStateChange == PlayModeStateChange.EnteredEditMode)
                UpdateManager.Instance.Clear();
        }
    }
}
