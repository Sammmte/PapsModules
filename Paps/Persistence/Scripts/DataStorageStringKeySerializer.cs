using System;
using Unity.Serialization.Json;

namespace Paps.Persistence
{
    internal class DataStorageStringKeySerializer : DataStorageKeySerializer<string>
    {
        public override string Deserialize(in SerializedMemberView memberView)
        {
            return memberView.Name().ToString();
        }

        public override void Serialize(string key, in JsonSerializationContext<DataStorage<string>> context)
        {
            context.Writer.WriteKey(key);
        }
    }
}