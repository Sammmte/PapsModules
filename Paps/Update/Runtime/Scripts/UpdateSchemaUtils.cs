using System;
using System.Collections.Generic;

namespace Paps.Update
{
    public static class UpdateSchemaUtils
    {
        public static bool TryGetGroupByName(string name, List<UpdateSchemaGroup> groups, out UpdateSchemaGroup group)
        {
            for(int i = 0; i < groups.Count; i++)
            {
                if(groups[i] != null && groups[i].name == name)
                {
                    group = groups[i];
                    return true;
                }
            }

            group = null;
            return false;
        }
    }
}