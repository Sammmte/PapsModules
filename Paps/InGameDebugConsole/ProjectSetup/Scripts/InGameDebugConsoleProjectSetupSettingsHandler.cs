using Paps.ProjectSetup;
using System;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Paps.InGameDebugConsole.ProjectSetup
{
    public class InGameDebugConsoleProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/InGameDebugConsole";
        private static readonly string PREFAB_VARIANT_PATH = Path.Combine(BASE_PATH, "InGameDebugConsoleVariant.prefab");

        public Type SettingsType => typeof(InGameDebugConsoleProjectSetupSettings);

        public int Order => 1;

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var originalPrefabGuid = AssetDatabase.FindAssets("InGameDebugConsolePrefab").First();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(originalPrefabGuid));
            var variant = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.SaveAsPrefabAsset(variant, PREFAB_VARIANT_PATH);
            GameObject.DestroyImmediate(variant);
            var variantGuid = AssetDatabase.GUIDFromAssetPath(PREFAB_VARIANT_PATH).ToString();
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group = settings.groups.First(g => g.Name == "Cheats");
            var entry = settings.CreateOrMoveEntry(variantGuid, group);
            entry.address = "InGameDebugConsolePrefab";
        }
    }
}
