using System;
using Unity.Serialization.Json;

namespace Paps.Persistence
{
    internal class DataStorageGuidKeySerializer : DataStorageKeySerializer<Guid>
    {
        public override Guid Deserialize(in SerializedMemberView memberView)
        {
            var nameView = memberView.Name();
            Span<char> nameViewSpan = stackalloc char[nameView.Length()];
            
            for(int i = 0; i < nameViewSpan.Length; i++)
            {
                nameViewSpan[i] = nameView[i];
            }

            return Guid.Parse(nameViewSpan);
        }

        public override void Serialize(Guid key, in JsonSerializationContext<DataStorage<Guid>> context)
        {
            context.Writer.WriteKey(key.ToString());
        }
    }
}