using System;
using System.Collections.Generic;

namespace Paps.Update
{
    public static class UpdateSchemaUtils
    {
        public static int GetIdOfGroup(UpdatableGroup group)
        {
            if(group.Equals(UpdatableGroup.NONE))
            {
                throw new InvalidOperationException($"group is == NONE. Cannot provide an id");
            }

            return group.GetHashCode();
        }

        public static bool TryGetGroupById(int id, List<UpdatableGroup> groups, out UpdatableGroup group)
        {
            for(int i = 0; i < groups.Count; i++)
            {
                var hashcode = groups[i].GetHashCode();

                if(id == hashcode)
                {
                    group = groups[i];
                    return true;
                }
            }

            group = UpdatableGroup.NONE;
            return false;
        }

        public static int GetId(this UpdatableGroup group) => GetIdOfGroup(group);
    }
}