using Paps.Build;
using Paps.LevelSetup.Editor;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine;

namespace Paps.LevelSetup.Cheats.Editor
{
    public class RemoveTestLevelsWhenNoCheats : IBuildPreprocessor
    {
        private const string CHEATS_DEFINE = "CHEATS";

        public void Process(BuildSettings currentBuildSettings)
        {
            if(!currentBuildSettings.GetDefineSymbols().Contains(CHEATS_DEFINE))
                RemoveTestLevels(currentBuildSettings);
        }

        private void RemoveTestLevels(BuildSettings buildSettings)
        {
            var testLevelScenes = EditorLevelManager.GetLevels()
                .Where(l => l.IsTestLevel)
                .SelectMany(l => l.GetRelatedScenes())
                .Select(s => s.GetSceneAssetPath());

            var productionLevelScenes = EditorLevelManager.GetLevels()
                .Where(l => !l.IsTestLevel)
                .SelectMany(l => l.GetRelatedScenes())
                .Select(s => s.GetSceneAssetPath());

            var scenesToRemove = testLevelScenes.Except(productionLevelScenes).ToArray();

            foreach(var scenePath in scenesToRemove)
                buildSettings.RemoveScenePath(scenePath);

            Debug.Log($"Removed test level scenes {JsonSerialization.ToJson(scenesToRemove)}");
        }
    }
}
