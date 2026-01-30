using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/Update Update Schema")]
    public class UpdateUpdateSchema : UpdateSchema<IUpdatable>
    {
        protected override void ExecuteUpdatesFor(IReadOnlyList<int> groupIds, IReadOnlyDictionary<int, UpdateList<IUpdatable>> updatableGroups)
        {
            var count = groupIds.Count;

            for(int i = 0; i < count; i++)
            {
                var updatables = updatableGroups[groupIds[i]];

                foreach (var updatable in updatables)
                {
                    try
                    {
                        updatable.ManagedUpdate();
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