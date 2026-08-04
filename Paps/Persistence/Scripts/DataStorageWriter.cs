namespace Paps.Persistence
{
    public class DataStorageWriter<TKey>
    {
        private TKey _key;
        private DataStorage<TKey> _dataStorage;

        public void Prepare(TKey key, DataStorage<TKey> dataStorage)
        {
            _key = key;
            _dataStorage = dataStorage;
        }

        public void Write<TValue>(TValue value)
        {
            _dataStorage.Set(_key, value);
        }

        public void Clear()
        {
            _key = default;
            _dataStorage = null;
        }
    }
}