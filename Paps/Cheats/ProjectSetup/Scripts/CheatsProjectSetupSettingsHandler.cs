using Paps.ProjectSetup;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor;
using System.Linq;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Paps.Cheats.ProjectSetup
{
    public class CheatsProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        public Type SettingsType => typeof(CheatsProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            var defaultStyleSheetGuid = AssetDatabase.FindAssets("UnityDefaultRuntimeTheme").First();
            var cheatsPrefabGuid = AssetDatabase.FindAssets("CheatsUIDocument").First();
            var cheatsSubmenuButtonGuid = AssetDatabase.FindAssets("CheatSubmenuButton").First();
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group = settings.CreateGroup("Cheats", false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            settings.CreateOrMoveEntry(defaultStyleSheetGuid, group);
            var cheatsPrefabEntry = settings.CreateOrMoveEntry(cheatsPrefabGuid, group);
            cheatsPrefabEntry.address = "CheatsPrefab";
            var cheatsSubmenuButtonEntry = settings.CreateOrMoveEntry(cheatsSubmenuButtonGuid, group);
            cheatsSubmenuButtonEntry.address = "CheatSubmenuButton";
        }
    }
}
