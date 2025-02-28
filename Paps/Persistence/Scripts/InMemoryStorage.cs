using Cysharp.Threading.Tasks;

namespace Paps.Persistence
{
    public class InMemoryStorage : IStorage
    {
        private string _data;

        public bool HasData => _data != null;

        public async UniTask<string> LoadAsync()
        {
            return _data;
        }

        public async UniTask SaveAsync(string data)
        {
            _data = data;
        }
    }
}