using System;

namespace Paps.ProjectSetup
{
    public interface IProjectSetupSettingsHandler
    {
        public Type SettingsType { get; }
        public int Order { get => 0; }
        public void HandleSettings(object customSettings);
    }
}