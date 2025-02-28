using Cysharp.Threading.Tasks;

namespace Paps.Persistence
{
    public interface IStorage
    {
        public bool HasData { get; }
        public UniTask<string> LoadAsync();
        public UniTask SaveAsync(string data);
    }
}