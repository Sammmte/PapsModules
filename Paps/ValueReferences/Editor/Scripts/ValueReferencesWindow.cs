using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferencesWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _windowTreeAsset;
        [SerializeField] private VisualTreeAsset _pathElementTreeAsset;
        [SerializeField] private VisualTreeAsset _groupElementTreeAsset;
        [SerializeField] private VisualTreeAsset _valueReferenceElementTreeAsset;

        private VisualElement _mainContainer;

        private ValueReferenceGroupAsset[] _groups;
        private PathTree<ValueReferenceGroupAsset[]> _pathTree;
        private PathTree<ValueReferencePathElement> _pathElementsTree;
        private ValueReferenceGroupAsset _orphanGroupAsset;

        [MenuItem("Paps/Value References/Manager Window")]
        public static void Display()
        {
            var window = EditorWindow.GetWindow<ValueReferencesWindow>("Value References Manager");

            window.Show(true);
        }

        private void CreateGUI()
        {
            _groups = GetGroups();
            _pathTree = CreateAndFillPathTree(_groups);
            _pathElementsTree = _pathTree.Map(node =>
            {
                var pathElementParent = _pathElementTreeAsset.CloneTree();
                var pathElement = pathElementParent.Q<ValueReferencePathElement>();

                return pathElement;
            });

            _pathElementsTree.Traverse(node =>
            {
                var pathElement = node.Data;

                pathElement.Initialize(node.Name, 
                    ValueReferencesUtils.GetGroupsForPath(_groups, node.GetPath()), 
                    node.Children.Select(c => c.Data).ToArray(),
                    _groupElementTreeAsset, _valueReferenceElementTreeAsset);
            });

            var windowVisualElement = _windowTreeAsset.CloneTree();

            _mainContainer = windowVisualElement.Q("MainContainer");

            _mainContainer.Add(_pathElementsTree.Root.Data);

            rootVisualElement.Add(windowVisualElement);
        }

        private ValueReferenceGroupAsset[] GetGroups()
        {
            IEnumerable<ValueReferenceGroupAsset> groupAssets = ValueReferencesUtils.GetGroupAssets();

            _orphanGroupAsset = ScriptableObject.CreateInstance<ValueReferenceGroupAsset>();
            _orphanGroupAsset.name = "Orphan Values";
            _orphanGroupAsset.Path = "ORPHAN";
            _orphanGroupAsset.ValueReferenceAssets = ValueReferencesUtils.GetOrphanValueReferenceAssets(groupAssets.ToArray());

            groupAssets = groupAssets.Append(_orphanGroupAsset);

            return groupAssets.ToArray();
        }

        private PathTree<ValueReferenceGroupAsset[]> CreateAndFillPathTree(ValueReferenceGroupAsset[] groups)
        {
            var pathTree = ValueReferencesUtils.GetGroupsPathTree(_groups);

            pathTree.Traverse(node =>
            {
                var groupsForNode = ValueReferencesUtils.GetGroupsForPath(_groups, node.GetPath());

                node.Data = groupsForNode;
            });

            return pathTree;
        }

        private void OnDestroy()
        {
            _pathElementsTree.Traverse(node =>
            {
                node.Data.Dispose();
            });

            ScriptableObject.DestroyImmediate(_orphanGroupAsset);
        }

    }
}
