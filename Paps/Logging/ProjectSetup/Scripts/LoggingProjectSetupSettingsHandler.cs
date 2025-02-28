using Paps.ProjectSetup;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace Paps.Logging.ProjectSetup
{
    public class LoggingProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/Logging";
        private static readonly string LOG_CONFIGURATION_PATH = Path.Combine(BASE_PATH, "LogConfiguration.asset");

        public Type SettingsType => typeof(LoggingProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group = settings.CreateGroup("Logging", false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            var newAsset = ScriptableObject.CreateInstance<LogConfiguration>();
            AssetDatabase.CreateAsset(newAsset, LOG_CONFIGURATION_PATH);
            var guid = AssetDatabase.GUIDFromAssetPath(LOG_CONFIGURATION_PATH).ToString();
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = "LogConfiguration";
        }
    }
}
