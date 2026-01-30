using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/FixedUpdate Update Schema")]
    public class FixedUpdateUpdateSchema : UpdateSchema<IFixedUpdatable>
    {
        protected override void ExecuteUpdatesFor(IReadOnlyList<int> groupIds, IReadOnlyDictionary<int, UpdateList<IFixedUpdatable>> updatableGroups)
        {
            var count = groupIds.Count;

            for(int i = 0; i < count; i++)
            {
                var updatables = updatableGroups[groupIds[i]];

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