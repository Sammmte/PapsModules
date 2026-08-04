using System;
using System.Collections.Generic;
using Unity.Serialization.Json;
using UnityEngine;

namespace Paps.Persistence
{
    public static class DataStorageSerializationHelper
    {
        public const string TYPE_DISCRIMINATOR_KEY = "type-discriminator";
        public const string VALUE_KEY = "value";

        private const string INT_TYPE_DISCRIMINATOR = "int";
        private const string FLOAT_TYPE_DISCRIMINATOR = "float";
        private const string BOOL_TYPE_DISCRIMINATOR = "bool";
        private const string STRING_TYPE_DISCRIMINATOR = "string";
        private const string GUID_TYPE_DISCRIMINATOR = "guid";
        private const string DATA_STORAGE_GUID_DISCRIMINATOR = "data-storage-guid";
        private const string DATA_STORAGE_STRING_DISCRIMINATOR = "data-storage-string";

        private static Dictionary<Type, object> _jsonSerializationAdapters = new Dictionary<Type, object>();
        private static Dictionary<Type, string> _entryValueDiscriminatorsByType = new Dictionary<Type, string>();
        private static Dictionary<string, DataStorageEntrySerializer> _entrySerializersByTypeDiscriminator = new Dictionary<string, DataStorageEntrySerializer>();

        static DataStorageSerializationHelper()
        {
            RegisterDefaultKeySerializers();
            RegisterDefaultEntryValueTypes();
        }

        private static void RegisterDefaultKeySerializers()
        {
            RegisterDataStorageKeySerializer(new DataStorageGuidKeySerializer());
            RegisterDataStorageKeySerializer(new DataStorageStringKeySerializer());
        }

        private static void RegisterDefaultEntryValueTypes()
        {
            RegisterDataStorageEntryValueType<int>(INT_TYPE_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<float>(FLOAT_TYPE_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<bool>(BOOL_TYPE_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<string>(STRING_TYPE_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<Guid>(GUID_TYPE_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<DataStorage<Guid>>(DATA_STORAGE_GUID_DISCRIMINATOR);
            RegisterDataStorageEntryValueType<DataStorage<string>>(DATA_STORAGE_STRING_DISCRIMINATOR);
        }

        internal static void RegisterDataStorageWithKey<TKey>()
        {
            var type = typeof(DataStorage<TKey>);

            if(!_jsonSerializationAdapters.ContainsKey(type))
            {
                var serializer = new DataStorageJsonAdapter<TKey>();
                _jsonSerializationAdapters[type] = serializer;

                JsonSerialization.AddGlobalAdapter(serializer);
            }
        }

        public static void RegisterDataStorageEntryValueType<T>(string typeDiscriminator)
        {
            var type = typeof(DataStorageEntry<T>);

            if(!_jsonSerializationAdapters.ContainsKey(type))
            {
                var serializer = new DataStorageEntrySerializer<T>(typeDiscriminator);

                _jsonSerializationAdapters[type] = serializer;
                _entrySerializersByTypeDiscriminator[typeDiscriminator] = serializer;
                _entryValueDiscriminatorsByType[type] = typeDiscriminator;
            }
        }

        public static void RegisterDataStorageKeySerializer<TKey>(DataStorageKeySerializer<TKey> serializer)
        {
            var type = typeof(TKey);

            _jsonSerializationAdapters[type] = serializer;
        }

        public static bool IsDataStorageKeyTypeRegistered<TKey>() => _jsonSerializationAdapters.ContainsKey(typeof(TKey));
        public static bool IsDataStorageEntryValueTypeRegistered<T>() => _jsonSerializationAdapters.ContainsKey(typeof(DataStorageEntry<T>));

        internal static bool TrySerializeKey<TKey>(TKey key, in JsonSerializationContext<DataStorage<TKey>> context)
        {
            if(!IsDataStorageKeyTypeRegistered<TKey>())
            {
                Debug.LogWarning($"Type {typeof(TKey).Name} is not registered as DataStorage key");
                return false;
            }

            var serializer = (DataStorageKeySerializer<TKey>)_jsonSerializationAdapters[typeof(TKey)];

            serializer.Serialize(key, in context);

            return true;
        }

        internal static bool TryDeserializeKey<TKey>(in SerializedMemberView memberView, out TKey key)
        {
            if(!IsDataStorageKeyTypeRegistered<TKey>())
            {
                Debug.LogWarning($"Type {typeof(TKey).Name} is not registered as DataStorage key");
                key = default;
                return false;
            }

            var serializer = (DataStorageKeySerializer<TKey>)_jsonSerializationAdapters[typeof(TKey)];

            key = serializer.Deserialize(in memberView);

            return true;
        }

        internal static bool TrySerializeEntry<TKey>(DataStorageEntry entry, in JsonSerializationContext<DataStorage<TKey>> context)
        {
            if(_entryValueDiscriminatorsByType.TryGetValue(entry.ValueType, out var typeDiscriminator))
            {
                var serializer = _entrySerializersByTypeDiscriminator[typeDiscriminator];

                serializer.Serialize(entry, in context);

                return true;
            }

            Debug.LogWarning($"Type {entry.ValueType.Name} is not registered as DataStorage type");
            return false;
        }

        internal static bool TryDeserializeEntry<TKey>(string typeDiscriminator, in SerializedValueView valueView, 
            in JsonDeserializationContext<DataStorage<TKey>> context, out DataStorageEntry entry)
        {
            if(_entrySerializersByTypeDiscriminator.TryGetValue(typeDiscriminator, out var serializer))
            {
                entry = serializer.Deserialize(in valueView, in context);
                return true;
            }

            Debug.LogWarning($"Type with discriminator {typeDiscriminator} is not registered as DataStorage type");

            entry = default;
            return false;
        }
    }
}