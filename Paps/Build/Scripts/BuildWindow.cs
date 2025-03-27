using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Build
{
    public class BuildWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private VisualTreeAsset _customSettingsElement;

        private Toggle _productionToggle;
        private EnumField _buildTargetEnumField;
        private EnumFlagsField _buildOptionsField;
        private Button _buildButton;
        private VisualElement _customSettingsContainer;

        private List<IBuildWindowSettings> _customBuildSettings = new List<IBuildWindowSettings>();

        [MenuItem("Paps/Build/Open Build Path/Development")]
        public static void OpenBuildPathDevelopment()
        {
            EditorUtility.RevealInFinder($"Build/Development/{Application.productName}.exe");
        }

        [MenuItem("Paps/Build/Open Build Path/Production")]
        public static void OpenBuildPathProduction()
        {
            EditorUtility.RevealInFinder($"Build/Production/{Application.productName}.exe");
        }

        [MenuItem("Paps/Build/Build Window")]
        public static void ShowExample()
        {
            BuildWindow window = GetWindow<BuildWindow>(utility: true);
            window.titleContent = new GUIContent("Build");
        }

        private void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            _productionToggle = rootVisualElement.Q<Toggle>("ProductionToggle");
            _buildTargetEnumField = rootVisualElement.Q<EnumField>("BuildTargetField");
            _buildOptionsField = rootVisualElement.Q<EnumFlagsField>("BuildOptionsField");
            _buildButton = rootVisualElement.Q<Button>("BuildButton");
            _customSettingsContainer = rootVisualElement.Q("CustomSettingsContainer");

            var existingCustomBuildSettings = TypeCache.GetTypesDerivedFrom<IBuildWindowSettings>().ToArray();

            _buildOptionsField.Init(BuildOptions.None);
            
            foreach(var customBuildSettingsType in existingCustomBuildSettings)
            {
                var customBuildSettingsElementTemplate = _customSettingsElement.CloneTree();

                var customBuildSettingsElement = customBuildSettingsElementTemplate.Q<CustomBuildSettingsElement>();
                var customBuildSettingsInstance = (IBuildWindowSettings)Activator.CreateInstance(customBuildSettingsType);
                customBuildSettingsElement.Initialize(customBuildSettingsInstance);
                _customSettingsContainer.Add(customBuildSettingsElement);
                _customBuildSettings.Add(customBuildSettingsInstance);
            }

            _buildButton.clicked += Build;
        }

        private void Build()
        {
            var settings = new BuildSettings()
            {
                OutputPath = "Build/" + (_productionToggle.value ? "Production" : "Development" ) + $"/{Application.productName}.exe",
                BuildTarget = (BuildTarget)_buildTargetEnumField.value,
                BuildOptions = (BuildOptions)_buildOptionsField.value
            };

            settings.SetScenePaths(EditorBuildSettings.scenes.Select(s => s.path).ToArray());

            if (_productionToggle.value)
                settings.AddDefineSymbol("PRODUCTION");
            else
                settings.AddDefineSymbol("DEVELOPMENT");

            foreach (var buildSetting in _customBuildSettings)
            {
                settings.AddSettings(buildSetting.GetSettingsObject());
            }

            Builder.Build(settings);

            EditorUtility.RevealInFinder(settings.OutputPath);

            Close();
        }
    }
}