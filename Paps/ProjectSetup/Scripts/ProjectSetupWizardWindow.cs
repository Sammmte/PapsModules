using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ProjectSetup
{
    public class ProjectSetupWizardWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private VisualTreeAsset _customSettingsElement;

        private Button _setupButton;
        private VisualElement _customSettingsContainer;

        private List<IProjectSetupWindowSettings> _customProjectSetupSettings = new List<IProjectSetupWindowSettings>();

        [MenuItem("Paps/Project Setup/Project Setup Wizard Window")]
        public static void ShowExample()
        {
            var window = GetWindow<ProjectSetupWizardWindow>(utility: true);
            window.titleContent = new GUIContent("Project Setup Wizard");
        }

        private void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            _setupButton = rootVisualElement.Q<Button>("SetupButton");
            _customSettingsContainer = rootVisualElement.Q("CustomSettingsContainer");

            var existingCustomProjectSetupSettings = TypeCache.GetTypesDerivedFrom<IProjectSetupWindowSettings>().ToArray();

            foreach (var customProjectSetupSettingsType in existingCustomProjectSetupSettings)
            {
                var customProjectSetupSettingsElementTemplate = _customSettingsElement.CloneTree();

                var customProjectSetupSettingsElement = customProjectSetupSettingsElementTemplate.Q<CustomProjectSetupSettingsElement>();
                var customProjectSetupSettingsInstance = (IProjectSetupWindowSettings)Activator.CreateInstance(customProjectSetupSettingsType);
                customProjectSetupSettingsElement.Initialize(customProjectSetupSettingsInstance);
                _customSettingsContainer.Add(customProjectSetupSettingsElement);
                _customProjectSetupSettings.Add(customProjectSetupSettingsInstance);
            }

            _setupButton.clicked += Setup;
        }

        private void Setup()
        {
            var customProjectSetupSettingsObjects = _customProjectSetupSettings.Select(s => s.GetSettingsObject())
                .ToArray();

            ProjectSetupper.SetupProject(customProjectSetupSettingsObjects);

            Close();
        }
    }
}