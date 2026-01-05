using UnityEditor;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor.VersionControl;

namespace Paps.ValueReferences.Editor
{
    public static class ValueReferencesUtils
    {
        public static ValueReferenceAsset[] GetOrphanValueReferenceAssets(ValueReferenceGroupAsset[] groups)
        {
            var allValueReferenceAssets = AssetDatabase.FindAssets($"t:{nameof(ValueReferenceAsset)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ValueReferenceAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            var allGroupedAssets = groups.SelectMany(g => g.ValueReferenceAssets);

            var orphanAssets = allValueReferenceAssets.Where(asset => !allGroupedAssets.Contains(asset)).ToArray();

            return orphanAssets;
        }

        public static ValueReferenceGroupAsset[] GetGroupAssets()
        {
            return AssetDatabase.FindAssets($"t:{nameof(ValueReferenceGroupAsset)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ValueReferenceGroupAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        public static PathTree<ValueReferenceGroupAsset[]> GetGroupsPathTree(ValueReferenceGroupAsset[] groups)
        {
            var uniquePaths = groups.Select(g => g.Path)
                .Distinct()
                .ToArray();

            return PathTree<ValueReferenceGroupAsset[]>.BuildFromPaths(uniquePaths);
        }

        public static ValueReferenceGroupAsset[] GetGroupsForPath(ValueReferenceGroupAsset[] groups, string path)
        {
            return groups.Where(g => g.Path == path)
                .ToArray();
        }
    }
}
