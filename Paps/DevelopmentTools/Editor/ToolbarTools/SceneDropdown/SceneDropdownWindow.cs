using Paps.SceneLoading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    public class SceneDropdownWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _windowVisualTree;
        [SerializeField] private VisualTreeAsset _sceneElementVisualTree;
        [SerializeField] private VisualTreeAsset _levelElementVisualTree;

        private void OnEnable()
        {
            _windowVisualTree.CloneTree(rootVisualElement);

            var windowContainer = rootVisualElement.Q("WindowContainer");

            var levels = EditorLevelManager.GetLevels()
                .OrderBy(l => l.Name);
            var levelScenes = levels.SelectMany(l =>
            {
                IEnumerable<Scene> allScenes = l.Level.InitialScenesGroup.Scenes.ToArray();

                if (l.LevelEditorData != null)
                    allScenes = allScenes.Concat(l.LevelEditorData.ExtraScenes.Scenes);

                return allScenes;
            });
            var staticScenes = EditorBuildSettings.scenes
                .Select(scene => scene.path)
                .Where(scenePath => !levelScenes.Any(scene => scenePath.Contains(scene.Path)))
                .OrderBy(scenePath => Path.GetFileNameWithoutExtension(scenePath));

            var staticScenesScrollView = rootVisualElement.Q<ScrollView>("StaticScenesScrollView");
            var levelsScrollView = rootVisualElement.Q<ScrollView>("LevelScenesScrollView");

            foreach (var scenePath in staticScenes)
            {
                var sceneElementTemplate = _sceneElementVisualTree.Instantiate();
                var sceneElement = sceneElementTemplate.Q<SceneElement>();
                sceneElement.Initialize(scenePath);
                staticScenesScrollView.Add(sceneElementTemplate);
            }

            foreach (var levelWithExtraData in levels)
            {
                var levelElementTemplate = _levelElementVisualTree.Instantiate();
                var sceneElement = levelElementTemplate.Q<LevelElement>();
                sceneElement.Initialize(levelWithExtraData.Level, levelWithExtraData.LevelEditorData);
                levelsScrollView.Add(levelElementTemplate);
            }
        }
    }
}
