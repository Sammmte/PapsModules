using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Paps.GameSettings
{
    public class InMemoryGameSettingsStorage : IGameSettingsStorage
    {
        private Dictionary<string, GameSettingSaveInfo> _data = new Dictionary<string, GameSettingSaveInfo>();

        public async UniTask<Dictionary<string, GameSettingSaveInfo>> Load(CancellationToken cancellationToken)
        {
            return _data;
        }

        public async UniTask Save(Dictionary<string, GameSettingSaveInfo> gameSettings)
        {
            _data = gameSettings;
        }
    }
}
