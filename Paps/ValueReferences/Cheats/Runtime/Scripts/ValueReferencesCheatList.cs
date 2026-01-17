using Paps.UnityExtensions;
using System;
using System.Linq;
using UnityEngine;

namespace Paps.ValueReferences.Cheats
{
    public class ValueReferencesCheatList : ScriptableObject
    {
        public const string ORPHAN_GROUP_NAME = "Orphan";

        [SerializeField] public ValueReferenceGroupAsset[] RawGroupAssets;
        [SerializeField] public ValueReferenceAsset[] RawOrphanAssets;

        [NonSerialized] private ValueReferenceGroupAsset[] _groups;
        private Tree<ValueReferenceTreeNodeData> _pathTree;

        public Tree<ValueReferenceTreeNodeData> GetPathTree()
        {
            if(_pathTree == null)
            {
                CreatePathTree();
            }

            return _pathTree;
        }

        private void CreatePathTree()
        {
            var groups = GetGroupAssets();

            var uniquePaths = groups.Select(g => g.Path)
                .Distinct()
                .ToArray();

            _pathTree = Tree<ValueReferenceTreeNodeData>.BuildFromPaths(uniquePaths);

            _pathTree.Traverse(node =>
            {
                var groups = GetGroupsForPath(node.GetPath());

                var treeNodeData = new ValueReferenceTreeNodeData()
                {
                    ValueReferenceAssets = groups.SelectMany(g => g.ValueReferenceAssets).ToArray()
                };

                node.Data = treeNodeData;
            });
        }

        public ValueReferenceGroupAsset[] GetGroupsForPath(string path)
        {
            return GetGroupAssets().Where(g => g.Path == path)
                .ToArray();
        }

        private ValueReferenceGroupAsset[] GetGroupAssets()
        {
            if(_groups == null)
            {
                CreateGroups();
            }

            return _groups;
        }

        private void CreateGroups()
        {
            _groups = RawGroupAssets.Append(CreateOrphanGroup()).ToArray();
        }

        private ValueReferenceGroupAsset CreateOrphanGroup()
        {
            var allGroupedAssets = RawGroupAssets.SelectMany(g => g.ValueReferenceAssets);

            var allValueReferenceAssets = allGroupedAssets.Concat(RawOrphanAssets);

            var orphanGroup = ScriptableObject.CreateInstance<ValueReferenceGroupAsset>();

            orphanGroup.name = ORPHAN_GROUP_NAME;
            orphanGroup.Path = ORPHAN_GROUP_NAME;
            orphanGroup.ValueReferenceAssets = RawOrphanAssets.ToArray();

            return orphanGroup;
        }
    }
}
