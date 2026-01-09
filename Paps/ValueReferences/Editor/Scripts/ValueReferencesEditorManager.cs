using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Paps.ValueReferences.Editor
{
    public static class ValueReferencesEditorManager
    {
        public const string ORPHAN_GROUP_NAME = "Orphan";
        public const string ORPHAN_GROUP_PATH_NAME = "VALUE_REFERENCES_ORPHAN_GROUP_PATH";

        private static ValueReferenceGroupAsset[] _groupAssets;
        private static Dictionary<string, ValueReferenceAsset> _valueReferencesWithPaths;
        private static Dictionary<ValueReferenceAsset, string> _pathsOfValueReferenceAssets;
        private static PathTree<ValueReferenceGroupAsset[]> _groupsPathTree;
        private static (CreateAssetMenuAttribute Attribute, Type Type)[] _createAssetMenuAttributesPerType;

        public static event Action OnRefresh;

        private static void LoadGroupAssets()
        {
            var groups = AssetDatabase.FindAssets($"t:{nameof(ValueReferenceGroupAsset)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ValueReferenceGroupAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            var orphanGroup = CreateOrphanGroup(groups);

            _groupAssets = groups.Append(orphanGroup).ToArray();
        }

        private static ValueReferenceGroupAsset CreateOrphanGroup(ValueReferenceGroupAsset[] existingGroups)
        {
            var allValueReferenceAssets = AssetDatabase.FindAssets($"t:{nameof(ValueReferenceAsset)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ValueReferenceAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            var allGroupedAssets = existingGroups.SelectMany(g => g.ValueReferenceAssets);

            var orphanAssets = allValueReferenceAssets.Where(asset => !allGroupedAssets.Contains(asset)).ToArray();

            var orphanGroup = ScriptableObject.CreateInstance<ValueReferenceGroupAsset>();

            orphanGroup.name = ORPHAN_GROUP_NAME;
            orphanGroup.Path = ORPHAN_GROUP_PATH_NAME;
            orphanGroup.ValueReferenceAssets = orphanAssets.ToArray();

            return orphanGroup;
        }

        public static ValueReferenceGroupAsset[] GetGroupAssets()
        {
            if(_groupAssets == null)
                LoadGroupAssets();

            return _groupAssets;
        }

        public static PathTree<ValueReferenceGroupAsset[]> GetGroupsPathTree()
        {
            if(_groupsPathTree == null)
            {
                LoadGroupsPathTree();
            }

            return _groupsPathTree;
        }

        public static PathTree<ValueReferenceGroupAsset[]> GetGroupsPathTreeContainingType<T>()
        {
            var groups = GetGroupAssets();

            var uniquePaths = groups
                .Where(g => g.ValueReferenceAssets.Any(v => v is IValueReferenceSource<T>))
                .Select(g => g.Path)
                .Distinct()
                .ToArray();

            var pathTree = PathTree<ValueReferenceGroupAsset[]>.BuildFromPaths(uniquePaths);

            pathTree.Traverse(node =>
            {
                node.Data = GetGroupsForPath(node.GetPath());
            });

            return pathTree;
        }

        private static void LoadGroupsPathTree()
        {
            var groups = GetGroupAssets();

            var uniquePaths = groups.Select(g => g.Path)
                .Distinct()
                .ToArray();

            _groupsPathTree = PathTree<ValueReferenceGroupAsset[]>.BuildFromPaths(uniquePaths);

            _groupsPathTree.Traverse(node =>
            {
                node.Data = GetGroupsForPath(node.GetPath());
            });
        }

        public static ValueReferenceGroupAsset[] GetGroupsForPath(string path)
        {
            return GetGroupAssets().Where(g => g.Path == path)
                .ToArray();
        }

        private static void LoadValueReferenceAssetsWithFullPaths()
        {
            var groups = GetGroupAssets();

            var assetsWithPath = groups.SelectMany(g =>
            {
                var tuples = new (string Path, ValueReferenceAsset Asset)[g.ValueReferenceAssets.Length];

                for(int i = 0; i < g.ValueReferenceAssets.Length; i++)
                {
                    tuples[i] = ($"{g.Path}/{g.ValueReferenceAssets[i].name}", g.ValueReferenceAssets[i]);
                }

                return tuples;
            });

            _valueReferencesWithPaths = assetsWithPath
                .ToDictionary(tuple => tuple.Path, tuple => tuple.Asset);

            _pathsOfValueReferenceAssets = assetsWithPath
                .ToDictionary(tuple => tuple.Asset, tuple => tuple.Path);
        }

        public static Dictionary<string, ValueReferenceAsset> GetValueReferenceAssetsWithFullPaths()
        {
            if(_valueReferencesWithPaths == null)
            {
                LoadValueReferenceAssetsWithFullPaths();
            }

            return _valueReferencesWithPaths;
        }

        public static string GetPath(this ValueReferenceAsset asset)
        {
            if(_pathsOfValueReferenceAssets == null)
            {
                LoadValueReferenceAssetsWithFullPaths();
            }

            return _pathsOfValueReferenceAssets[asset];
        }

        public static (CreateAssetMenuAttribute Attribute, Type Type)[] GetCreateAssetMenuPerType()
        {
            if(_createAssetMenuAttributesPerType == null)
            {
                LoadCreateAssetMenuPerType();
            }

            return _createAssetMenuAttributesPerType;
        }

        private static void LoadCreateAssetMenuPerType()
        {
            _createAssetMenuAttributesPerType = TypeCache.GetTypesDerivedFrom<ValueReferenceAsset>()
                .Where(t => !t.IsAbstract)
                .Select(t => (Attribute: t.GetCustomAttribute<CreateAssetMenuAttribute>(), Type: t))
                .Where(tuple => tuple.Attribute != null)
                .ToArray();
        }

        public static void RefreshAll()
        {
            _groupAssets = null;
            _valueReferencesWithPaths = null;
            _pathsOfValueReferenceAssets = null;
            _groupsPathTree = null;
            _createAssetMenuAttributesPerType = null;

            OnRefresh?.Invoke();
        }

        public static void RefreshGroups()
        {
            _groupAssets = null;
            _valueReferencesWithPaths = null;
            _pathsOfValueReferenceAssets = null;
            _groupsPathTree = null;

            OnRefresh?.Invoke();
        }

        public static void RefreshPaths()
        {
            _valueReferencesWithPaths = null;
            _pathsOfValueReferenceAssets = null;
            _groupsPathTree = null;

            OnRefresh?.Invoke();
        }

        private static Dictionary<ValueReferenceGroupAsset, ValueReferenceAsset[]> GetGroupsContainingAssets(ValueReferenceAsset[] assets)
        {
            var groupsContainingAssets = _groupAssets.Where(g => g.ValueReferenceAssets.Any(v => assets.Contains(v)));

            return groupsContainingAssets.ToDictionary(g => g, g => g.ValueReferenceAssets.Where(v => assets.Contains(v)).ToArray());
        }

        public static void MoveValueReferencesAssets(ValueReferenceAsset[] assets, ValueReferenceGroupAsset targetGroup)
        {
            var assetAlreadyAddedToGroup = assets.Where(a => targetGroup.ValueReferenceAssets.Contains(a)).ToArray();

            if(assetAlreadyAddedToGroup.Length != 0)
            {
                string message = $"Target group {targetGroup.name} already contains ValueReferenceAsset assets: ";

                for(int i = 0; i < assetAlreadyAddedToGroup.Length; i++)
                {
                    var asset = assetAlreadyAddedToGroup[i];

                    message += $"{asset.name}";

                    if(i != assetAlreadyAddedToGroup.Length - 1)
                    {
                        message += ", ";
                    }
                }

                Debug.Log(message);
            }

            assets = assets.Where(a => !assetAlreadyAddedToGroup.Contains(a)).ToArray();

            if(assets.Length == 0)
                return;

            var groupsWithAssets = GetGroupsContainingAssets(assets);

            foreach(var group in groupsWithAssets)
            {
                var serializedObject = new SerializedObject(group.Key);

                var arrayProperty = serializedObject.FindProperty(nameof(ValueReferenceGroupAsset.ValueReferenceAssets));

                serializedObject.Update();

                for(int i = arrayProperty.arraySize - 1; i >= 0; i--)
                {
                    var elementProperty = arrayProperty.GetArrayElementAtIndex(i);

                    if(assets.Contains(elementProperty.objectReferenceValue))
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                serializedObject.ApplyModifiedProperties();

                serializedObject.Dispose();
            }

            var targetGroupSerializedObject = new SerializedObject(targetGroup);

            var targetGroupArrayProperty = targetGroupSerializedObject.FindProperty(nameof(ValueReferenceGroupAsset.ValueReferenceAssets));

            targetGroupSerializedObject.Update();

            for(int i = 0; i < assets.Length; i++)
            {
                var newIndex = targetGroupArrayProperty.arraySize;

                targetGroupArrayProperty.InsertArrayElementAtIndex(newIndex);

                var newProperty = targetGroupArrayProperty.GetArrayElementAtIndex(newIndex);

                newProperty.objectReferenceValue = assets[i];
            }

            targetGroupSerializedObject.ApplyModifiedProperties();

            targetGroupSerializedObject.Dispose();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            RefreshAll();
        }
    }
}
