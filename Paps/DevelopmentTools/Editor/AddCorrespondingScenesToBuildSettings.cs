using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    [InitializeOnLoad]
    public static class AddCorrespondingScenesToBuildSettings
    {
        private const string ENTRY_SCENE_NAME = "Entry", SETUP_SCENE_NAME = "Setup";
        private const string SCENES_PATH = "Assets/Game";

        static AddCorrespondingScenesToBuildSettings()
        {
            EditorApplication.projectChanged += CheckIfUpdateIsNeeded;

            CheckIfUpdateIsNeeded();
        }

        private static void CheckIfUpdateIsNeeded()
        {
            var projectScenes = GetProjectScenes();

            if(!ContainsMandatoryScenes(projectScenes))
            {
                Debug.LogWarning("Mandatory scenes are missing");
                return;
            }

            var correspondingScenes = FilterAndRearrangeScenes(projectScenes);

            if (ShouldUpdate(correspondingScenes))
            {
                Update(correspondingScenes);
            }
        }

        private static bool ContainsMandatoryScenes(List<string> projectScenes)
        {
            return projectScenes.Any(path => Path.GetFileNameWithoutExtension(path) == ENTRY_SCENE_NAME) && 
                projectScenes.Any(path => Path.GetFileNameWithoutExtension(path) == SETUP_SCENE_NAME);
        }

        private static void Update(string[] correspondingScenes)
        {
            EditorBuildSettings.scenes = correspondingScenes
                .Select(scenePath => new EditorBuildSettingsScene(scenePath, true))
                .ToArray();

            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        private static bool ShouldUpdate(string[] correspondingScenes)
        {
            return EditorBuildSettings.scenes.Length != correspondingScenes.Length ||
                !EditorBuildSettings.scenes.All(scene => correspondingScenes.Contains(scene.path));
        }

        private static string[] FilterAndRearrangeScenes(List<string> projectScenes)
        {
            EnsureEntrySceneIsFirstScene(projectScenes);
            Filter(projectScenes);

            return projectScenes.ToArray();
        }

        private static List<string> GetProjectScenes() => AssetDatabase.FindAssets("t:scene", new[] { SCENES_PATH })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .ToList();

        private static void EnsureEntrySceneIsFirstScene(List<string> scenePaths)
        {
            var entryScenePath = scenePaths.Find(path => path.Contains(ENTRY_SCENE_NAME));

            var index = scenePaths.IndexOf(entryScenePath);

            if(index == -1)
                return;

            if (index != 0)
            {
                var sceneAtIndexZero = scenePaths[0];
                scenePaths[0] = entryScenePath;
                scenePaths[index] = sceneAtIndexZero;
            }
        }

        private static void Filter(List<string> scenePaths)
        {
            var filterScenes = new List<string>();

            filterScenes.AddRange(scenePaths.Where(path => path.ToLower().Contains("template")));
            filterScenes.AddRange(scenePaths.Where(path => path.ToLower().Contains("ignore")));
            filterScenes.AddRange(scenePaths.Where(path => path.ToLower().Contains("legacy")));

            scenePaths.RemoveAll(path => filterScenes.Contains(path));
        }
    }
}