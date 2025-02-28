using Cysharp.Threading.Tasks;
using System.IO;

namespace Paps.Persistence
{
    public class FileStorage : IStorage
    {
        private string _path;

        public bool HasData => File.Exists(_path);

        public FileStorage(string path)
        {
            _path = path;
        }

        public async UniTask<string> LoadAsync()
        {
            return await File.ReadAllTextAsync(_path);
        }

        public async UniTask SaveAsync(string data)
        {
            await File.WriteAllTextAsync(_path, data);
        }
    }
}