using Paps.SceneLoading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.LevelSetup.Editor
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
                .OrderBy(l => l.Id);
            var levelScenes = levels.SelectMany(l =>
            {
                IEnumerable<Scene> scenes = new Scene[0];

                if(l.InitialScenesGroup != null)
                    scenes = l.InitialScenesGroup;

                if(l.ExtraScenes != null)
                    scenes = scenes.Concat(l.ExtraScenes);

                return scenes;
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

            foreach (var level in levels)
            {
                var levelElementTemplate = _levelElementVisualTree.Instantiate();
                var sceneElement = levelElementTemplate.Q<LevelElement>();
                sceneElement.Initialize(level);
                levelsScrollView.Add(levelElementTemplate);
            }
        }
    }
}
