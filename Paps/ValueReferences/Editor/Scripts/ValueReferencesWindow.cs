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

        private VisualElement _mainContainer;
        private VisualElement _pathElementsContainer;
        private Button _refreshUIButton;
        private Button _refreshDataButton;
        private Button _refreshAllButton;

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
            RefreshUI();
        }

        private void OnBecameVisible()
        {
            RefreshUI();
        }

        private void RefreshAll()
        {
            RefreshUI();
            ValueReferencesEditorManager.RefreshAll();
        }

        private void RefreshUI()
        {
            CleanUp();
            rootVisualElement.Clear();

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
                    node.Children.Select(c => c.Data).ToArray());
            });

            var windowVisualElement = _windowTreeAsset.CloneTree();

            _mainContainer = windowVisualElement.Q("MainContainer");
            _pathElementsContainer = _mainContainer.Q("PathElementsContainer");
            _refreshUIButton = _mainContainer.Q<Button>("RefreshUIButton");
            _refreshDataButton = _mainContainer.Q<Button>("RefreshDataButton");
            _refreshAllButton = _mainContainer.Q<Button>("RefreshAllButton");

            _refreshUIButton.clicked += RefreshUI;
            _refreshDataButton.clicked += ValueReferencesEditorManager.RefreshAll;
            _refreshAllButton.clicked += RefreshAll;

            foreach(var child in _pathElementsTree.Root.Data.ChildPathElements)
            {
                _pathElementsContainer.Add(child);
            }

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
            CleanUp();
        }
        
        private void CleanUp()
        {
            if(_pathElementsTree == null)
                return;

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
