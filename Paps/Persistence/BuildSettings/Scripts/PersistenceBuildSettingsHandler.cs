using Paps.Build;
using System;

namespace Paps.Persistence.Build
{
    public class PersistenceBuildSettingsHandler : IBuildSettingsHandler
    {
        public Type SettingsType => typeof(PersistenceBuildSettings);

        public int Order => 0;

        public void HandleSettings(BuildSettings currentBuildSettings, object customSettings)
        {
            var persistenceSettings = customSettings as PersistenceBuildSettings;

            if (persistenceSettings.PersistenceEnabled)
                currentBuildSettings.AddDefineSymbol("PERSISTENCE");
        }
    }
}
