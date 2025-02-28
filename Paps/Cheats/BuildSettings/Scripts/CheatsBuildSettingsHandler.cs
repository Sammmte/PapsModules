using Paps.Build;
using System;

namespace Paps.Cheats.Build
{
    public class CheatsBuildSettingsHandler : IBuildSettingsHandler
    {
        public Type SettingsType => typeof(CheatsBuildSettings);

        public void HandleSettings(BuildSettings currentBuildSettings, object customSettings)
        {
            var cheatsBuildSettings = (CheatsBuildSettings)customSettings;

            if (cheatsBuildSettings.CheatsEnabled)
                currentBuildSettings.AddDefineSymbol("CHEATS");
            else
                currentBuildSettings.DontIncludeAddressablesGroup("Cheats");
        }
    }
}
