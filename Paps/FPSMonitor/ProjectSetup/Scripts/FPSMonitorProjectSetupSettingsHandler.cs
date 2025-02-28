using Paps.ProjectSetup;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEngine;

namespace Paps.FPSMonitor.ProjectSetup
{
    public class FPSMonitorProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/FPSMonitor";
        private static readonly string PREFAB_VARIANT_PATH = Path.Combine(BASE_PATH, "FPSMonitorVariant.prefab");

        public Type SettingsType => typeof(FPSMonitorProjectSetupSettings);

        public int Order => 1;

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var originalPrefabGuid = AssetDatabase.FindAssets("FPSMonitorPrefab").First();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(originalPrefabGuid));
            var variant = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.SaveAsPrefabAsset(variant, PREFAB_VARIANT_PATH);
            GameObject.DestroyImmediate(variant);
            var variantGuid = AssetDatabase.GUIDFromAssetPath(PREFAB_VARIANT_PATH).ToString();
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group = settings.groups.First(g => g.Name == "Cheats");
            var entry = settings.CreateOrMoveEntry(variantGuid, group);
            entry.address = "FPSMonitorPrefab";
        }
    }
}
