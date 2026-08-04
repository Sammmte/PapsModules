using System;
using Unity.Serialization.Json;

namespace Paps.Persistence
{
    public abstract class DataStorageKeySerializer<TKey>
    {
        public abstract void Serialize(TKey key, in JsonSerializationContext<DataStorage<TKey>> context);
        public abstract TKey Deserialize(in SerializedMemberView memberView);
    }
}