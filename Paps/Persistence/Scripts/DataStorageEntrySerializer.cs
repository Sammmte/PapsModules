using System;
using Unity.Serialization.Json;

namespace Paps.Persistence
{
    internal abstract class DataStorageEntrySerializer
    {
        public string TypeDiscriminator { get; }

        public DataStorageEntrySerializer(string typeDiscriminator)
        {
            TypeDiscriminator = typeDiscriminator;
        }

        public abstract DataStorageEntry Deserialize<TKey>(in SerializedValueView valueView, in JsonDeserializationContext<DataStorage<TKey>> context);
        public abstract void Serialize<TKey>(DataStorageEntry entry, in JsonSerializationContext<DataStorage<TKey>> context);
    }

    internal class DataStorageEntrySerializer<T> : DataStorageEntrySerializer
    {
        public DataStorageEntrySerializer(string typeDiscriminator) : base(typeDiscriminator)
        {
            
        }

        public override DataStorageEntry Deserialize<TKey>(in SerializedValueView valueView,  in JsonDeserializationContext<DataStorage<TKey>> context)
        {
            var entry = DataStorageEntry<T>.GetPooled();

            entry.Value = context.DeserializeValue<T>(valueView);

            return entry;
        }

        public override void Serialize<TKey>(DataStorageEntry entry, in JsonSerializationContext<DataStorage<TKey>> context)
        {
            var castedEntry = entry as DataStorageEntry<T>;

            context.Writer.WriteKeyValue(DataStorageSerializationHelper.TYPE_DISCRIMINATOR_KEY, TypeDiscriminator);
            context.SerializeValue(DataStorageSerializationHelper.VALUE_KEY, castedEntry.Value);
        }
    }
}