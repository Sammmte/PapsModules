using Paps.ObjectPooling;

namespace Paps.Persistence
{
    public partial class DataStorage<TKey>
    {
        private static ObjectPool<DataStorage<TKey>> _dataStoragePool;
        public static int GlobalCreationCapacity { get; set; }

        static DataStorage()
        {
            DataStorageSerializationHelper.RegisterDataStorageWithKey<TKey>();
        }

        private static ObjectPool<DataStorage<TKey>> GetPool()
        {
            if(_dataStoragePool == null)
            {
                _dataStoragePool = new ObjectPool<DataStorage<TKey>>(CreateDataStorage);
            }

            return _dataStoragePool;
        }

        public static void PreparePoolAmount(int amount, bool prewarm = true)
        {
            if(_dataStoragePool == null)
            {
                _dataStoragePool = new ObjectPool<DataStorage<TKey>>(CreateDataStorage, capacity: amount);

                _dataStoragePool.Prewarm();
            }
            else
            {
                _dataStoragePool.Capacity += amount;
                
                if(prewarm)
                {
                    _dataStoragePool.Prewarm();
                }
            }
        }

        public static void PreparePoolAmountForValueOfType<T>(int amount, bool prewarm = true)
        {
            DataStorageEntry<T>.PreparePoolAmount(amount, prewarm);
        }

        private static DataStorage<TKey> CreateDataStorage() => new DataStorage<TKey>(GlobalCreationCapacity);

        public static DataStorage<TKey> Rent() => GetPool().Get();

        public static void Return(DataStorage<TKey> dataStorage)
        {
            dataStorage.Clear();

            GetPool().Release(dataStorage);
        }
    }
}
