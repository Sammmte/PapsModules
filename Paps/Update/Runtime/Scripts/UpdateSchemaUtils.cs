using System;
using System.Collections.Generic;

namespace Paps.Update
{
    public static class UpdateSchemaUtils
    {
        public const string DEFAULT_GROUP = "Default";

        public static int GetIdOfGroup(string group)
        {
            if(string.IsNullOrEmpty(group))
            {
                throw new InvalidOperationException($"group is null or empty. Cannot provide an id");
            }

            return group.GetHashCode();
        }

        public static bool TryGetGroupById(int id, List<string> groups, out string group)
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

            group = string.Empty;
            return false;
        }

        public static int GetId(this string group) => GetIdOfGroup(group);
    }
}