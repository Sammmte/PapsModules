using Paps.Build;
using Paps.LevelSetup.Editor;
using System.Linq;
using UnityEditor;

namespace Paps.LevelSetup.Cheats.Editor
{
    public class PrepareCheatLevelsOnCallbacks : IBuildPreprocessor
    {   
        [InitializeOnLoadMethod]
        public static void ListenCallbacks()
        {
            EditorApplication.playModeStateChanged += PrepareLevelsForPlayMode;
        }

        private static void PrepareLevelsForPlayMode(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.ExitingEditMode)
                UpdateLevelsCheatListAsset();
        }

        private static void UpdateLevelsCheatListAsset()
        {
            var guid = AssetDatabase.FindAssets($"t:{nameof(LevelsCheatListAsset)}").FirstOrDefault();
            if (guid == null)
                return;

            var listAsset = AssetDatabase.LoadAssetAtPath<LevelsCheatListAsset>(AssetDatabase.GUIDToAssetPath(guid));

            var levelAssets = EditorLevelManager.GetLevels();

            listAsset.Levels = levelAssets.ToArray();
            
            EditorUtility.SetDirty(listAsset);
            AssetDatabase.SaveAssets();
        }

        public void Process(BuildSettings currentBuildSettings)
        {
            UpdateLevelsCheatListAsset();
        }
    }
}
