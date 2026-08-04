using System.Collections.Generic;

namespace Paps.Persistence
{
    public partial class DataStorage<TKey>
    {
        private Dictionary<TKey, DataStorageEntry> _entries;

        public bool IsEmpty => _entries.Count == 0;

        private DataStorage(int capacity)
        {
            _entries = new Dictionary<TKey, DataStorageEntry>(capacity);
        }

        public bool Contains(TKey key) => _entries.ContainsKey(key);

        internal void SetInternal(TKey key, DataStorageEntry entry) => _entries[key] = entry;
        internal DataStorageEntry GetInternal(TKey key) => _entries[key];
        internal bool TryGetInternal(TKey key, out DataStorageEntry entry) => _entries.TryGetValue(key, out entry);
        internal bool RemoveInternal(TKey key) => RemoveInternal(key, out _);
        internal bool RemoveInternal(TKey key, out DataStorageEntry entry) => _entries.Remove(key, out entry);

        internal Dictionary<TKey, DataStorageEntry>.Enumerator GetEnumerator() => _entries.GetEnumerator();

        

        public T Get<T>(TKey key)
        {
            return ((DataStorageEntry<T>) GetInternal(key)).Value;
        }

        public bool TryGet<T>(TKey key, out T result)
        {
            if(TryGetInternal(key, out var entry) && entry is DataStorageEntry<T> castedEntry)
            {
                result = castedEntry.Value;
                return true;
            }

            result = default;
            return false;
        }

        public void Set<T>(TKey key, T value)
        {
            if(TryGetInternal(key, out var entry))
            {
                if(entry is DataStorageEntry<T> castedEntry)
                {
                    castedEntry.Value = value;
                }
                else
                {
                    RemoveInternal(key);
                    entry.Release();

                    var newEntry = DataStorageEntry<T>.GetPooled(value);

                    newEntry.Value = value;

                    SetInternal(key, newEntry);
                }
            }
            else
            {
                var newEntry = DataStorageEntry<T>.GetPooled(value);

                newEntry.Value = value;

                SetInternal(key, newEntry);
            }
        }

        public void Remove(TKey key)
        {
            if(RemoveInternal(key, out var entry))
            {
                entry.Release();
            }
        }

        public void Clear()
        {
            foreach(var keyValue in _entries)
            {
                keyValue.Value.Release();
            }

            _entries.Clear();
        }

        public void Return() => Return(this);
    }
}