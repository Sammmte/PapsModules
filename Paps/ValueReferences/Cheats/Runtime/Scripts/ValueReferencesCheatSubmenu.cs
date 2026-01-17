using Cysharp.Threading.Tasks;
using Paps.Cheats;
using Paps.Optionals;
using Paps.RuntimeInspector.Cheats;
using Paps.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Cheats
{
    public class ValueReferencesCheatSubmenu : ICheatSubmenu
    {
        private const string GROUP_NAME_LABEL_CLASS = "value-reference-cheat-submenu__item__group-name-element";
        private const string VALUE_NAME_LABEL_CLASS = "value-reference-cheat-submenu__item__value-name-element";
        private const string TYPE_NAME_LABEL_CLASS = "value-reference-cheat-submenu__item__type-name-element";
        private const string VALUE_BUTTON_CONTAINER_CLASS = "value-reference-cheat-submenu__item__value-button-container";

        public enum SearchOptions
        {
            ByName,
            ByType
        }

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
        private TextField _searchField;
        private EnumField _searchOptionField;

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
            _searchField = _mainContainer.Q<TextField>("SearchField");
            _searchOptionField = _mainContainer.Q<EnumField>("SearchOptionField");

            _searchField.RegisterValueChangedCallback(ev => LoadTreeViewBySearch(ev.newValue));
            _searchOptionField.RegisterValueChangedCallback(ev =>
            {
                if(!string.IsNullOrEmpty(_searchField.value))
                    LoadTreeViewBySearch(_searchField.value);
            });
        }

        private void LoadTreeViewBySearch(string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                LoadDefaultTreeView();
            }
            else
            {
                var option = (SearchOptions)_searchOptionField.value;

                switch(option)
                {
                    case SearchOptions.ByName:
                        LoadSearchByNameMatchTreeView(search);
                        break;
                    
                    case SearchOptions.ByType:
                        LoadSearchByTypeNameMatchTreeView(search);
                        break;
                }
            }

            _multiColumnTreeView.Rebuild();
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
            LoadDefaultTreeView();

            _multiColumnTreeView.columns["Name"].makeCell += CreateNameCell;
            _multiColumnTreeView.columns["Type"].makeCell += CreateTypeCell;
            _multiColumnTreeView.columns["Value"].makeCell += CreateValueCell;

            _multiColumnTreeView.columns["Name"].bindCell += BindNameCell;
            _multiColumnTreeView.columns["Type"].bindCell += BindTypeCell;
            _multiColumnTreeView.columns["Value"].bindCell += BindValueCell;
        }

        private void LoadDefaultTreeView()
        {
            var items = new List<TreeViewItemData<ValueReferenceOrGroupPath>>();

            var id = 0;
            InitializeDefaultTreeViewNodeRecursively(_pathTree.Root, items, ref id);

            _multiColumnTreeView.SetRootItems(items);
        }

        private void InitializeDefaultTreeViewNodeRecursively(TreeNode<ValueReferenceTreeNodeData> node, 
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
                
                InitializeDefaultTreeViewNodeRecursively(child, subItems, ref id);
                var treeViewItem = new TreeViewItemData<ValueReferenceOrGroupPath>(thisChildId, new ValueReferenceOrGroupPath(child.Name), subItems);
                items.Add(treeViewItem);
            }
        }

        private void LoadSearchByNameMatchTreeView(string searchMatch)
        {
            var items = new List<TreeViewItemData<ValueReferenceOrGroupPath>>();

            var id = 0;
            InitializeSearchByNameMatchTreeViewNodeRecursively(_pathTree.Root, items, ref id, searchMatch, false);

            _multiColumnTreeView.SetRootItems(items);
        }

        private void InitializeSearchByNameMatchTreeViewNodeRecursively(TreeNode<ValueReferenceTreeNodeData> node, 
            List<TreeViewItemData<ValueReferenceOrGroupPath>> items, ref int id, string searchMatch, bool includeAllChilds)
        {
            var groupNameLowerCase = node.Name.ToLower();

            if(groupNameLowerCase.Contains(searchMatch))
                includeAllChilds = true;

            if(node.Data.ValueReferenceAssets.Length > 0)
            {
                foreach(var valueReferenceAsset in node.Data.ValueReferenceAssets)
                {
                    var lowerCaseName = valueReferenceAsset.name.ToLower();
                    if(!includeAllChilds && !lowerCaseName.Contains(searchMatch))
                        continue;

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
                
                InitializeSearchByNameMatchTreeViewNodeRecursively(child, subItems, ref id, searchMatch, includeAllChilds);

                if(!includeAllChilds && subItems.Count == 0)
                    continue;
                
                var treeViewItem = new TreeViewItemData<ValueReferenceOrGroupPath>(thisChildId, new ValueReferenceOrGroupPath(child.Name), subItems);
                items.Add(treeViewItem);
            }
        }

        private void LoadSearchByTypeNameMatchTreeView(string searchMatch)
        {
            var items = new List<TreeViewItemData<ValueReferenceOrGroupPath>>();

            var id = 0;
            InitializeSearchByTypeNameMatchTreeViewNodeRecursively(_pathTree.Root, items, ref id, searchMatch);

            _multiColumnTreeView.SetRootItems(items);
        }

        private void InitializeSearchByTypeNameMatchTreeViewNodeRecursively(TreeNode<ValueReferenceTreeNodeData> node, 
            List<TreeViewItemData<ValueReferenceOrGroupPath>> items, ref int id, string searchMatch)
        {
            if(node.Data.ValueReferenceAssets.Length > 0)
            {
                foreach(var valueReferenceAsset in node.Data.ValueReferenceAssets)
                {
                    var valueProperty = valueReferenceAsset.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

                    var lowerCaseTypeName = GetEffectiveTypeSearchName(valueProperty.PropertyType).ToLower();

                    if(!lowerCaseTypeName.Contains(searchMatch))
                        continue;

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
                
                InitializeSearchByTypeNameMatchTreeViewNodeRecursively(child, subItems, ref id, searchMatch);

                if(subItems.Count == 0)
                    continue;
                
                var treeViewItem = new TreeViewItemData<ValueReferenceOrGroupPath>(thisChildId, new ValueReferenceOrGroupPath(child.Name), subItems);
                items.Add(treeViewItem);
            }
        }
        
        private string GetEffectiveTypeSearchName(Type type)
        {
            var customDisplay = GetCustomDisplayFor(type);

            if(customDisplay.HasValue)
                return customDisplay.Value.DisplayName;

            return type.Name;
        }

        private VisualElement CreateNameCell()
        {
            var nameLabel = new Label();

            foreach(var styleSheet in _submenuVTA.stylesheets)
            {
                nameLabel.styleSheets.Add(styleSheet);
            }

            return nameLabel;
        }

        private VisualElement CreateTypeCell()
        {
            var typeLabel = new Label();

            foreach(var styleSheet in _submenuVTA.stylesheets)
            {
                typeLabel.styleSheets.Add(styleSheet);
            }

            typeLabel.AddToClassList(TYPE_NAME_LABEL_CLASS);

            return typeLabel;
        }

        private VisualElement CreateValueCell()
        {
            var valueCell = new ValueCell();

            foreach(var styleSheet in _submenuVTA.stylesheets)
            {
                valueCell.styleSheets.Add(styleSheet);
            }

            valueCell.AddToClassList(VALUE_BUTTON_CONTAINER_CLASS);

            return valueCell;
        }

        private void BindNameCell(VisualElement element, int index)
        {
            var label = element as Label;
            var data = _multiColumnTreeView.GetItemDataForIndex<ValueReferenceOrGroupPath>(index);

            if(data.IsGroup)
            {
                label.RemoveFromClassList(VALUE_NAME_LABEL_CLASS);
                label.AddToClassList(GROUP_NAME_LABEL_CLASS);
                label.text = data.GroupPathName;
            }
            else
            {
                label.RemoveFromClassList(GROUP_NAME_LABEL_CLASS);
                label.AddToClassList(VALUE_NAME_LABEL_CLASS);
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

                var customDisplay = GetCustomDisplayFor(valueProperty.PropertyType);

                if(customDisplay.HasValue)
                {
                    label.text = customDisplay.Value.DisplayName;
                    label.style.color = customDisplay.Value.Color;
                }
                else
                {
                    label.text = valueProperty.PropertyType.Name;
                    label.style.color = Color.black;
                }
            }
        }

        private Optional<ValueReferenceTypeCustomDisplay> GetCustomDisplayFor(Type type)
        {
            if(type == typeof(int))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Int",
                    Color = Color.cyan,
                };
            }

            if(type == typeof(float))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Float",
                    Color = Color.cyan,
                };
            }

            if(type == typeof(bool))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Boolean",
                    Color = Color.yellow,
                };
            }

            if(type == typeof(string))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "String",
                    Color = Color.red,
                };
            }

            return default;
        }

        private void BindValueCell(VisualElement element, int index)
        {
            var valueCell = element as ValueCell;

            var data = _multiColumnTreeView.GetItemDataForIndex<ValueReferenceOrGroupPath>(index);

            valueCell.SetData(data);
        }
    }
}
