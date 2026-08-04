using Paps.ObjectPooling;
using System;

namespace Paps.Persistence
{
    public abstract class DataStorageEntry
    {
        internal Type ValueType { get; }

        internal DataStorageEntry(Type valueType)
        {
            ValueType = valueType;
        }
        internal abstract void Release();
    }

    public class DataStorageEntry<T> : DataStorageEntry
    {
        private static ObjectPool<DataStorageEntry<T>> _pool;

        private static ObjectPool<DataStorageEntry<T>> GetPool()
        {
            if(_pool == null)
            {
                _pool = new ObjectPool<DataStorageEntry<T>>(Create);
            }

            return _pool;
        }

        public static void PreparePoolAmount(int amount, bool prewarm = true)
        {
            if(_pool == null)
            {
                _pool = new ObjectPool<DataStorageEntry<T>>(Create, capacity: amount);
            }
            else
            {
                _pool.Capacity += amount;
            }

            if(prewarm)
            {
                _pool.Prewarm();
            }
        }

        private static DataStorageEntry<T> Create() => new DataStorageEntry<T>();

        internal static DataStorageEntry<T> GetPooled(T value = default)
        {
            var pooled = GetPool().Get();

            pooled.Value = value;

            return pooled;
        }

        internal T Value;

        internal DataStorageEntry() : base(typeof(DataStorageEntry<T>))
        {
            
        }

        internal override void Release()
        {
            GetPool().Release(this);
        }
    }
}