using Cysharp.Threading.Tasks;
using Paps.Cheats;
using Paps.RuntimeInspector.Cheats;
using Paps.UnityExtensions;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Cheats
{
    public class ValueReferencesCheatSubmenu : ICheatSubmenu
    {
        private struct ValueReferenceOrGroupPath
        {
            public ValueReferenceAsset ValueReferenceAsset;
            public string GroupPathName;

            public bool IsGroup => ValueReferenceAsset == null;

            public ValueReferenceOrGroupPath(ValueReferenceAsset asset)
            {
                ValueReferenceAsset = asset;
                GroupPathName = null;
            }

            public ValueReferenceOrGroupPath(string groupPathName)
            {
                ValueReferenceAsset = null;
                GroupPathName = groupPathName;
            }
        }

        private class ValueCell : VisualElement
        {
            private Button _inspectButton;

            private ValueReferenceOrGroupPath _currentData;

            public ValueCell()
            {
                _inspectButton = new Button();
                _inspectButton.text = "Inspect";
                _inspectButton.clicked += Inspect;

                Add(_inspectButton);
            }

            public void SetData(ValueReferenceOrGroupPath data)
            {
                _currentData = data;

                if(_currentData.IsGroup)
                    ShowAsGroup();
                else
                    ShowAsValue();
            }

            private void ShowAsGroup()
            {
                _inspectButton.style.display = DisplayStyle.None;
            }

            private void ShowAsValue()
            {
                _inspectButton.style.display = DisplayStyle.Flex;
            }

            private void Inspect()
            {
                RuntimeInspectorManager.Instance.Inspect(_currentData.ValueReferenceAsset);
            }
        }

        public string DisplayName => "Value References";

        private VisualTreeAsset _submenuVTA;
        private ValueReferencesCheatList _cheatList;

        private VisualElement _mainContainer;
        private VisualElement _noElementsLabelContainer;
        private VisualElement _elementsContainer;
        private MultiColumnTreeView _multiColumnTreeView;

        private Tree<ValueReferenceTreeNodeData> _pathTree;

        public VisualElement GetVisualElement()
        {
            return _mainContainer;
        }

        public async UniTask Load()
        {
            _submenuVTA = await this.LoadAssetAsync<VisualTreeAsset>("ValueReferencesCheatSubmenuUI");
            _cheatList = await this.LoadAssetAsync<ValueReferencesCheatList>("ValueReferencesCheatList");
            _pathTree = _cheatList.GetPathTree();

            InitializeUIElements();

            if(!HasAtLeastOneElement())
            {
                _noElementsLabelContainer.style.display = DisplayStyle.Flex;
                _elementsContainer.style.display = DisplayStyle.None;
                return;
            }

            InitializeTreeView();
        }

        private void InitializeUIElements()
        {
            _mainContainer = _submenuVTA.CloneTree();

            _noElementsLabelContainer = _mainContainer.Q("NoElementsContainer");
            _elementsContainer = _mainContainer.Q("ElementsContainer");
            _multiColumnTreeView = _mainContainer.Q<MultiColumnTreeView>();
        }

        private bool HasAtLeastOneElement()
        {
            var has = false;

            _pathTree.Traverse(node =>
            {
                if(node.Data.ValueReferenceAssets.Length > 0)
                {
                    has = true;
                }
            });

            return has;
        }

        private void InitializeTreeView()
        {
            var items = new List<TreeViewItemData<ValueReferenceOrGroupPath>>();

            var id = 0;
            InitializeTreeNodeRecursively(_pathTree.Root, items, ref id);

            _multiColumnTreeView.SetRootItems(items);

            _multiColumnTreeView.columns["Name"].makeCell += CreateNameCell;
            _multiColumnTreeView.columns["Type"].makeCell += CreateTypeCell;
            _multiColumnTreeView.columns["Value"].makeCell += CreateValueCell;

            _multiColumnTreeView.columns["Name"].bindCell += BindNameCell;
            _multiColumnTreeView.columns["Type"].bindCell += BindTypeCell;
            _multiColumnTreeView.columns["Value"].bindCell += BindValueCell;
        }

        private void InitializeTreeNodeRecursively(TreeNode<ValueReferenceTreeNodeData> node, 
            List<TreeViewItemData<ValueReferenceOrGroupPath>> items, ref int id)
        {
            if(node.Data.ValueReferenceAssets.Length > 0)
            {
                foreach(var valueReferenceAsset in node.Data.ValueReferenceAssets)
                {
                    id++;
                    var treeViewItem = new TreeViewItemData<ValueReferenceOrGroupPath>(id, new ValueReferenceOrGroupPath(valueReferenceAsset));
                    items.Add(treeViewItem);
                }
            }

            foreach(var child in node.Children)
            {
                id++;
                var thisChildId = id;
                var subItems = new List<TreeViewItemData<ValueReferenceOrGroupPath>>();
                
                InitializeTreeNodeRecursively(child, subItems, ref id);
                var treeViewItem = new TreeViewItemData<ValueReferenceOrGroupPath>(thisChildId, new ValueReferenceOrGroupPath(child.Name), subItems);
                items.Add(treeViewItem);
            }
        }

        private VisualElement CreateNameCell()
        {
            return new Label();
        }

        private VisualElement CreateTypeCell()
        {
            return new Label();
        }

        private VisualElement CreateValueCell()
        {
            return new ValueCell();
        }

        private void BindNameCell(VisualElement element, int index)
        {
            var label = element as Label;
            var data = _multiColumnTreeView.GetItemDataForIndex<ValueReferenceOrGroupPath>(index);

            if(data.IsGroup)
            {
                label.text = data.GroupPathName;
            }
            else
            {
                label.text = data.ValueReferenceAsset.name;
            }
        }

        private void BindTypeCell(VisualElement element, int index)
        {
            var label = element as Label;
            var data = _multiColumnTreeView.GetItemDataForIndex<ValueReferenceOrGroupPath>(index);

            if(data.IsGroup)
            {
                label.text = string.Empty;
            }
            else
            {
                var valueProperty = data.ValueReferenceAsset.GetType()
                    .GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                label.text = valueProperty.PropertyType.Name;
            }
        }

        private void BindValueCell(VisualElement element, int index)
        {
            var valueCell = element as ValueCell;

            var data = _multiColumnTreeView.GetItemDataForIndex<ValueReferenceOrGroupPath>(index);

            valueCell.SetData(data);
        }
    }
}
