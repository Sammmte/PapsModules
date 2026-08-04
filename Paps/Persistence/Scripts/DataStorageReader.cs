namespace Paps.Persistence
{
    public class DataStorageReader<TKey>
    {
        private TKey _key;
        private DataStorage<TKey> _dataStorage;

        public void Prepare(TKey key, DataStorage<TKey> dataStorage)
        {
            _key = key;
            _dataStorage = dataStorage;
        }

        public bool TryRead<TValue>(out TValue value)
        {
            return _dataStorage.TryGet(_key, out value);
        }

        public void Clear()
        {
            _key = default;
            _dataStorage = null;
        }
    }
}