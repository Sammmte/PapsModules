using Cysharp.Threading.Tasks;
using Paps.Persistence;
using System.Threading;

namespace Paps.GameSettings
{
    public class InMemoryGameSettingsStorage : IGameSettingsStorage
    {
        private DataStorage<string> _data;

        public async UniTask<DataStorage<string>> Load(CancellationToken cancellationToken)
        {
            return _data;
        }

        public async UniTask Save(DataStorage<string> gameSettings)
        {
            _data = gameSettings;
        }
    }
}
