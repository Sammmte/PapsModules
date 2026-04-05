using Paps.Build;
using System.Linq;
using UnityEditor;

namespace Paps.Levels.Editor
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
            var guid = AssetDatabase.FindAssets($"t:{nameof(LevelList)}").FirstOrDefault();
            if (guid == null)
                return;

            var listAsset = AssetDatabase.LoadAssetAtPath<LevelList>(AssetDatabase.GUIDToAssetPath(guid));

            var levelAssets = EditorLevelManager.GetLevels();

            listAsset.SetLevels(levelAssets.ToList());
            
            AssetDatabase.SaveAssets();
        }

        public void Process(BuildSettings currentBuildSettings)
        {
            UpdateLevelsCheatListAsset();
        }
    }
}
