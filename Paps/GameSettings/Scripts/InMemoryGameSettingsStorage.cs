using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Paps.GameSettings
{
    public class InMemoryGameSettingsStorage : IGameSettingsStorage
    {
        private Dictionary<string, GameSettingSaveInfo> _data = new Dictionary<string, GameSettingSaveInfo>();

        public async UniTask<Dictionary<string, GameSettingSaveInfo>> Load()
        {
            return _data;
        }

        public async UniTask Save(Dictionary<string, GameSettingSaveInfo> gameSettings)
        {
            _data = gameSettings;
        }
    }
}
