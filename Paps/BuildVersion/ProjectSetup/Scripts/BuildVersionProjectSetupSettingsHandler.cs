using Paps.ProjectSetup;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor;
using System.Linq;

namespace Paps.BuildVersion.ProjectSetup
{
    public class BuildVersionProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        public Type SettingsType => typeof(BuildVersionProjectSetupSettings);

        public int Order => 1;

        public void HandleSettings(object customSettings)
        {
            var guid = AssetDatabase.FindAssets("BuildVersionMenu").First();
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group = settings.groups.FirstOrDefault(g => g.Name == "Logging");
            group ??= settings.CreateGroup("Logging", false, false, false, null);
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = "BuildVersionMenu";
        }
    }
}
