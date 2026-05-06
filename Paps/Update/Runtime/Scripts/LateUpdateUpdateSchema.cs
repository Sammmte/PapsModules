using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/LateUpdate Update Schema")]
    public class LateUpdateUpdateSchema : UpdateSchema<ILateUpdatable>
    {
        protected override void ExecuteUpdatesFor(IReadOnlyList<UpdateSchemaGroup> groups, IReadOnlyDictionary<UpdateSchemaGroup, UpdateList<ILateUpdatable>> updatableGroups)
        {
            var count = groups.Count;

            for(int i = 0; i < count; i++)
            {
                var updatables = updatableGroups[groups[i]];

                foreach (var updatable in updatables)
                {
                    try
                    {
                        updatable.ManagedLateUpdate();
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