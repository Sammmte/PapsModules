using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = ValueReferenceAsset.BASE_CREATE_ASSET_MENU_PATH + "Group")]
    public class ValueReferenceGroupAsset : ScriptableObject
    {
        public struct ValueReferencePathInfo<T>
        {
            public string Path;
            public ValueReferenceAsset<T> ValueReferenceAsset;
        }
        
        [field: SerializeField] public ValueReferenceGroup Group { get; set; }

        public ValueReferencePathInfo<T>[] GetReferencesOfType<T>()
        {
            var list = new List<ValueReferencePathInfo<T>>();

            GetReferencesOfTypeFromGroup(Group, null, list);

            return list.ToArray();
        }

        private void GetReferencesOfTypeFromGroup<T>(ValueReferenceGroup valueReferenceGroup, string composedPath, List<ValueReferencePathInfo<T>> list)
        {
            if (composedPath == null)
            {
                composedPath = valueReferenceGroup.GroupPath;
            }
            else
            {
                composedPath += $"/{valueReferenceGroup.GroupPath}";
            }
            
            list.AddRange(valueReferenceGroup.ValueReferences.OfType<ValueReferenceAsset<T>>()
                .Select(v => new ValueReferencePathInfo<T>()
                {
                    Path = composedPath + $"/{v.name}",
                    ValueReferenceAsset = v
                }));

            foreach (var childGroup in valueReferenceGroup.SubGroups)
            {
                GetReferencesOfTypeFromGroup(childGroup, composedPath, list);
            }
        }
    }
}