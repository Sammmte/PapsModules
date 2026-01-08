using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferencesWindow : EditorWindow
    {
        private const string BASE_SAVE_KEY = "value-references-window";
        private const string SCROLL_POSITION_SAVE_KEY = BASE_SAVE_KEY + "scroll-position";

        [SerializeField] private VisualTreeAsset _windowTreeAsset;
        [SerializeField] private VisualTreeAsset _pathElementTreeAsset;

        private VisualElement _mainContainer;
        private ScrollView _pathElementsContainer;
        private Button _refreshButton;

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
            ValueReferencesEditorManager.OnRefresh += RefreshUI;
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

                pathElement.Initialize(node, 
                    ValueReferencesEditorManager.GetGroupsForPath(node.GetPath()), 
                    node.Children.Select(c => c.Data).ToArray());
            });

            var windowVisualElement = _windowTreeAsset.CloneTree();

            _mainContainer = windowVisualElement.Q("MainContainer");
            _pathElementsContainer = _mainContainer.Q<ScrollView>("PathElementsContainer");
            _refreshButton = _mainContainer.Q<Button>("RefreshButton");

            _refreshButton.clicked += RefreshAll;

            _pathElementsContainer.verticalScroller.slider.RegisterCallback<ChangeEvent<float>>(ev =>
            {
                SaveScrollPosition(ev.newValue);
            });

            foreach(var child in _pathElementsTree.Root.Data.ChildPathElements)
            {
                _pathElementsContainer.Add(child);
            }

            rootVisualElement.Add(windowVisualElement);

            LoadState();
        }

        private void LoadState()
        {
            var scrollPosition = ValueReferencesUIUtils.VALUE_REFERENCES_EDITOR_USER_PROJECT_PREFS.Get<float>(SCROLL_POSITION_SAVE_KEY);

            _pathElementsContainer.verticalScroller.slider.SetValueWithoutNotify(scrollPosition);
        }

        private void SaveScrollPosition(float scrollPosition)
        {
            ValueReferencesUIUtils.VALUE_REFERENCES_EDITOR_USER_PROJECT_PREFS.Set(SCROLL_POSITION_SAVE_KEY, scrollPosition);
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
    }
}
