using System.Collections.Generic;

namespace Paps.GameSettings
{
    public interface IDynamicGameSettingCreator
    {
        public KeyValuePair<string, GameSetting>[] Create();
    }
}
