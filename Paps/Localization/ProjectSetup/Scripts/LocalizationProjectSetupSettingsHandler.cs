using Paps.ProjectSetup;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Paps.Localization.ProjectSetup
{
    public class LocalizationProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/Localization";
        private static readonly string SETUP_ASSET_PATH = Path.Combine(BASE_PATH, "LocalizationGameSetupProcess.asset");

        public Type SettingsType => typeof(LocalizationProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var newSetupAsset = ScriptableObject.CreateInstance<LocalizationGameSetupProcess>();
            AssetDatabase.CreateAsset(newSetupAsset, SETUP_ASSET_PATH);
        }
    }
}
