using Paps.Build;
using System.Linq;
using UnityEditor;

namespace Paps.LevelSetup.Cheats.Editor
{
    public class PrepareCheatLevelsOnCallbacks : IBuildPreprocessor
    {
        private const string SCENES_PATH = "Assets/Game";
        
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

            var levelAssets = AssetDatabase.FindAssets($"t:{nameof(ScriptableLevel)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableLevel>(AssetDatabase.GUIDToAssetPath(guid)));

            listAsset.Levels = levelAssets.ToArray();
            
            EditorUtility.SetDirty(listAsset);
            AssetDatabase.SaveAssets();
        }

        public void Process(BuildSettings currentBuildSettings)
        {
            UpdateLevelsCheatListAsset();

            var currentScenes = currentBuildSettings.GetScenePaths();

            var finalScenes = currentScenes.Concat(GetProjectScenes().Where(path => !currentScenes.Contains(path)));
            
            currentBuildSettings.SetScenePaths(finalScenes.ToArray());
        }
        
        private static string[] GetProjectScenes() => AssetDatabase.FindAssets("t:scene", new[] { SCENES_PATH })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .ToArray();
    }
}
