using System;
using Unity.Serialization.Json;

namespace Paps.Persistence
{
    internal class DataStorageJsonAdapter<TKey> : IJsonAdapter<DataStorage<TKey>>
    {
        public DataStorage<TKey> Deserialize(in JsonDeserializationContext<DataStorage<TKey>> context)
        {
            var instance = DataStorage<TKey>.Rent();

            var objectView = context.SerializedValue.AsObjectView();

            var enumerator = objectView.GetEnumerator();

            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;

                DataStorageSerializationHelper.TryDeserializeKey<TKey>(current, out var key);

                var valueView = current.Value();
                var typeDiscriminator = valueView.GetValue(DataStorageSerializationHelper.TYPE_DISCRIMINATOR_KEY).AsStringView().ToString();

                var entryValueView = valueView.GetValue(DataStorageSerializationHelper.VALUE_KEY);

                DataStorageSerializationHelper.TryDeserializeEntry(typeDiscriminator, in entryValueView, in context, out var entry);

                instance.SetInternal(key, entry);
            }

            return instance;
        }

        public void Serialize(in JsonSerializationContext<DataStorage<TKey>> context, DataStorage<TKey> value)
        {
            using var writeObjectScope = context.Writer.WriteObjectScope();

            var enumerator = value.GetEnumerator();

            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;

                DataStorageSerializationHelper.TrySerializeKey(current.Key, in context);

                context.Writer.WriteBeginObject();
                DataStorageSerializationHelper.TrySerializeEntry(current.Value, in context);
                context.Writer.WriteEndObject();
            }
        }
    }
}