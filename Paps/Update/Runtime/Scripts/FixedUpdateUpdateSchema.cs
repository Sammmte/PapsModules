using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/FixedUpdate Update Schema")]
    public class FixedUpdateUpdateSchema : UpdateSchema<IFixedUpdatable>
    {
        protected override void ExecuteUpdatesFor(IReadOnlyList<UpdateSchemaGroup> groups, IReadOnlyDictionary<UpdateSchemaGroup, UpdateList<IFixedUpdatable>> updatableGroups)
        {
            var count = groups.Count;

            for(int i = 0; i < count; i++)
            {
                var updatables = updatableGroups[groups[i]];

                foreach (var updatable in updatables)
                {
                    try
                    {
                        updatable.ManagedFixedUpdate();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }
    }
}