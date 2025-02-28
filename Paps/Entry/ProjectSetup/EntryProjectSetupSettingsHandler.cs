using Paps.ProjectSetup;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace Paps.Entry.ProjectSetup
{
    public class EntryProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/Entry";
        private static readonly string SCENE_PATH = Path.Combine(BASE_PATH, "Entry.unity");

        public Type SettingsType => typeof(EntryProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            CreateSceneAssetWithEntryObject();
        }

        private void CreateSceneAssetWithEntryObject()
        {
            var guid = AssetDatabase.FindAssets("EntryPointObject").First();
            var entryObjectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SetActiveScene(scene);
            PrefabUtility.InstantiatePrefab(entryObjectPrefab, scene);
            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}
