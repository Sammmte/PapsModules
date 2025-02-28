using System;

namespace Paps.Build
{
    public interface IBuildSettingsHandler
    {
        public Type SettingsType { get; }
        public int Order { get => 0; }
        public void HandleSettings(BuildSettings currentBuildSettings, object customSettings);
    }
}