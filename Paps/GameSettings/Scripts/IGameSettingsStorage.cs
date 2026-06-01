using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Paps.GameSettings
{
    public interface IGameSettingsStorage
    {
        public UniTask Save(Dictionary<string, GameSettingSaveInfo> gameSettings);
        public UniTask<Dictionary<string, GameSettingSaveInfo>> Load(CancellationToken cancellationToken);
    }
}
