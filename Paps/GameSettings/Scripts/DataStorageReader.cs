using Paps.Persistence;

namespace Paps.GameSettings
{
    internal class DataStorageReader
    {
        private string _settingId;
        private DataStorage<string> _dataStorage;

        public void Prepare(string id, DataStorage<string> dataStorage)
        {
            _settingId = id;
            _dataStorage = dataStorage;
        }

        public bool TryRead<T>(out T value)
        {
            return _dataStorage.TryGet(_settingId, out value);
        }
    }
}
