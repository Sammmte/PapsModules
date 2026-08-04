using Paps.Persistence;

namespace Paps.GameSettings
{
    internal class DataStorageWriter
    {
        private string _settingId;
        private DataStorage<string> _dataStorage;

        public void Prepare(string id, DataStorage<string> dataStorage)
        {
            _settingId = id;
            _dataStorage = dataStorage;
        }

        public void Write<T>(T value)
        {
            _dataStorage.Set(_settingId, value);
        }
    }
}
