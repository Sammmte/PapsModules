using Paps.ProjectSetup;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Paps.GameSetup;

namespace Paps.StartupSetup.ProjectSetup
{
    public class StartupSetupProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/Setup";
        private static readonly string SCENE_PATH = Path.Combine(BASE_PATH, "Setup.unity");
        private static readonly string SETUP_PIPELINE_ASSET_PATH = Path.Combine(BASE_PATH, "GameSetupPipeline.asset");

        public Type SettingsType => typeof(StartupSetupProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var newPipeline = CreateGameSetupPipeline();
            CreateSceneWithObjects(newPipeline);
        }

        private GameSetupPipeline CreateGameSetupPipeline()
        {
            var newPipeline = ScriptableObject.CreateInstance<GameSetupPipeline>();
            AssetDatabase.CreateAsset(newPipeline, SETUP_PIPELINE_ASSET_PATH);

            return newPipeline;
        }

        private void CreateSceneWithObjects(GameSetupPipeline gameSetupPipelineAsset)
        {
            var guid = AssetDatabase.FindAssets("StartupSetupper").First();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            var startupSetupper = instance.GetComponent<StartupSetupper>();
            var serializedObject = new SerializedObject(startupSetupper);
            serializedObject.FindProperty("_setupPipeline").objectReferenceValue = gameSetupPipelineAsset;
            serializedObject.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}
