using System.IO;
using System.Linq;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Scripting.Editor
{
    public class AssemblyDefinitionCreationWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private AssemblyDefinitionCreationConfiguration _configuration;
        [SerializeField, HideInInspector] private AssemblyDefinitionAsset[] _assemblyReferences;

        private TextField _assemblyNamefield, _assemblyRootNamespaceField;
        private Toggle _customNamespaceToggle, _editorOnlyToggle;
        private PropertyField _assemblyReferencesField;
        private Button _createButton;

        private SerializedObject _serializedObject;
        private string _path;

        [MenuItem("Assets/Create/Paps/Assembly Definition", priority = -1000)]
        public static void ShowExample()
        {
            AssemblyDefinitionCreationWindow wnd = GetWindow<AssemblyDefinitionCreationWindow>(utility: true);
            wnd.titleContent = new GUIContent("Assembly Definition Creator");
            wnd.Initialize(GetCurrentSelectedDirectory());
        }

        private static string GetCurrentSelectedDirectory()
        {
            var selectedObject = Selection.activeObject;

            if (selectedObject == null)
                return null;

            var assetPath = AssetDatabase.GetAssetPath(selectedObject);

            if (AssetDatabase.IsValidFolder(assetPath))
                return assetPath;

            return null;
        }

        public void CreateGUI()
        {
            _serializedObject = new SerializedObject(this);
            _visualTreeAsset.CloneTree(rootVisualElement);

            _assemblyNamefield = rootVisualElement.Q<TextField>("AssemblyNameField");
            _customNamespaceToggle = rootVisualElement.Q<Toggle>("CustomNamespaceToggle");
            _assemblyRootNamespaceField = rootVisualElement.Q<TextField>("AssemblyRootNamespaceField");
            _editorOnlyToggle = rootVisualElement.Q<Toggle>("EditorOnlyToggle");
            _assemblyReferencesField = rootVisualElement.Q<PropertyField>("AssemblyReferencesField");
            _createButton = rootVisualElement.Q<Button>("CreateButton");

            _customNamespaceToggle.RegisterValueChangedCallback(ev =>
            {
                _assemblyRootNamespaceField.isReadOnly = !ev.newValue;
                if (!ev.newValue)
                    _assemblyRootNamespaceField.value = _assemblyNamefield.value;
            });

            _assemblyNamefield.RegisterValueChangedCallback(ev =>
            {
                if (!_customNamespaceToggle.value)
                    _assemblyRootNamespaceField.value = ev.newValue;
            });

            SetDefaultAssemblyReferences();
            _assemblyReferencesField.BindProperty(_serializedObject.FindProperty(nameof(_assemblyReferences)));

            _createButton.clicked += CreateAssemblyDefinitionAsset;
        }

        private void Initialize(string workingDirectory)
        {
            _path = workingDirectory;
        }

        private void SetDefaultAssemblyReferences()
        {
            _assemblyReferences = _configuration.DefaultAssemblyDefinitions;
        }

        private void CreateAssemblyDefinitionAsset()
        {
            var json = JsonSerialization.ToJson(new AssemblyDefinition()
            {
                name = _assemblyNamefield.value,
                rootNamespace = _assemblyRootNamespaceField.value,
                references = GetAssemblyReferencesAsStrings(),
                includePlatforms = _editorOnlyToggle.value ? new string[] { "Editor" } : new string[0],
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true,
                defineConstraints = new string[0],
                versionDefines = new string[0],
                noEngineReferences = false
            });

            File.WriteAllText(GetNewAssemblyDefinitionAssetPath(), json);
            AssetDatabase.Refresh();

            Close();
        }

        private string GetNewAssemblyDefinitionAssetPath() => Path.Combine(_path, _assemblyNamefield.value + ".asmdef");

        private string[] GetAssemblyReferencesAsStrings() => _assemblyReferences
            .Where(a => a != null)
            .Select(a => "GUID:" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(a)))
            .ToArray();
    }
}