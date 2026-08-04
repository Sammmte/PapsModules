using Cysharp.Threading.Tasks;
using Paps.Persistence;
using System.Threading;

namespace Paps.GameSettings
{
    public interface IGameSettingsStorage
    {
        public UniTask Save(DataStorage<string> gameSettings);
        public UniTask<DataStorage<string>> Load(CancellationToken cancellationToken);
    }
}
