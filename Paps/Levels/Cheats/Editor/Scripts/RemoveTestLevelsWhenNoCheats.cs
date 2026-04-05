using Paps.Build;
using Paps.Levels.Editor;
using System.Linq;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;

namespace Paps.Levels.Cheats.Editor
{
    public class RemoveTestLevelsWhenNoCheats : IBuildPreprocessor
    {
        private const string CHEATS_DEFINE = "CHEATS";

        public int Order => 10;

        public void Process(BuildSettings currentBuildSettings)
        {
            if(!currentBuildSettings.GetDefineSymbols().Contains(CHEATS_DEFINE))
                RemoveTestLevels(currentBuildSettings);
        }

        private void RemoveTestLevels(BuildSettings buildSettings)
        {
            var allLevels = EditorLevelManager.GetLevels();

            var testLevels = allLevels
                .Where(l => l.IsTestLevel)
                .ToArray();

            var testLevelScenes = testLevels
                .SelectMany(l => l.GetRelatedScenes())
                .Select(s => s.GetSceneAssetPath());

            var productionLevelScenes = EditorLevelManager.GetLevels()
                .Where(l => !l.IsTestLevel)
                .SelectMany(l => l.GetRelatedScenes())
                .Select(s => s.GetSceneAssetPath());

            var scenesToRemove = testLevelScenes.Except(productionLevelScenes).ToArray();

            foreach(var scenePath in scenesToRemove)
                buildSettings.RemoveScenePath(scenePath);

            var levelList = EditorLevelManager.GetLevelsList();

            var finalLevels = allLevels.Except(testLevels).ToList();

            levelList.SetLevels(finalLevels);

            AssetDatabase.SaveAssets();

            Debug.Log($"Removed test level scenes {JsonSerialization.ToJson(scenesToRemove)}");
            Debug.Log($"Final build levels {JsonSerialization.ToJson(finalLevels.Select(l => l.Id).ToArray())}");
        }
    }
}
