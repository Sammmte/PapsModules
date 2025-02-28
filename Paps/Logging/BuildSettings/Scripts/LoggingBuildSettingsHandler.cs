using Paps.Build;
using System;

namespace Paps.Logging.Build
{
    public class LoggingBuildSettingsHandler : IBuildSettingsHandler
    {
        public Type SettingsType => typeof(LoggingBuildSettings);

        public int Order => 0;

        public void HandleSettings(BuildSettings currentBuildSettings, object customSettings)
        {
            var loggingBuildSettings = (LoggingBuildSettings)customSettings;

            if (loggingBuildSettings.LogEnabled)
                currentBuildSettings.AddDefineSymbol("PAPS_LOG");
            else
                currentBuildSettings.DontIncludeAddressablesGroup("Logging");
        }
    }
}
