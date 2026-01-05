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

        [MenuItem("Paps/Value References/Manager Window")]
        public static void Display()
        {
            var window = EditorWindow.GetWindow<ValueReferencesWindow>("Value References Manager");

            window.Show(true);
        }

        private void CreateGUI()
        {
            _groups = GetGroups();
            _pathTree = GetPathTree();
            _pathElementsTree = _pathTree.Map(node =>
            {
                var pathElementParent = _pathElementTreeAsset.CloneTree();
                var pathElement = pathElementParent.Q<ValueReferencePathElement>();

                return pathElement;
            });

            _pathElementsTree.Traverse(node =>
            {
                var pathElement = node.Data;

                pathElement.Initialize(GetNodeName(node.Name), 
                    ValueReferencesEditorManager.GetGroupsForPath(node.GetPath()), 
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
            return ValueReferencesEditorManager.GetGroupAssets();
        }

        private PathTree<ValueReferenceGroupAsset[]> GetPathTree()
        {
            return ValueReferencesEditorManager.GetGroupsPathTree();
        }

        private void OnDestroy()
        {
            _pathElementsTree.Traverse(node =>
            {
                node.Data.Dispose();
            });
        }

        private string GetNodeName(string inputName)
        {
            if(inputName == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME)
                return "ORPHAN VALUES";

            return inputName;
        }

    }
}
