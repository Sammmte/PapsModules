using System;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferencesAdvancedDropdown : AdvancedDropdown
    {
        private PathTree<ValueReferenceGroupAsset[]> _pathTree;
        private Type _filterType;

        public event Action<ValueReferenceAsset> OnSelected;

        public ValueReferencesAdvancedDropdown(AdvancedDropdownState state, 
            PathTree<ValueReferenceGroupAsset[]> pathTree, Type filterType = null) : base(state)
        {
            _pathTree = pathTree;
            _filterType = filterType;

            minimumSize = ValueReferencesEditorConfig.Instance.MinimumAdvancedDropdownSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Value References");

            BuildRecursively(root, _pathTree.Root);

            return root;
        }

        private void BuildRecursively(AdvancedDropdownItem currentItem, TreeNode<ValueReferenceGroupAsset[]> node)
        {
            if(node.Data != null)
            {
                var valueReferenceAssets = node.Data.SelectMany(g => g.ValueReferenceAssets).ToArray();

                foreach(var asset in valueReferenceAssets)
                {
                    if(!IsAllowedType(asset))
                        continue;

                    currentItem.AddChild(new ValueReferenceAdvancedDropdownItem(asset.name, asset));
                }
            }

            foreach(var childNode in node.Children)
            {
                var newDropdownItem = new AdvancedDropdownItem(GetItemName(childNode.Name));
                currentItem.AddChild(newDropdownItem);

                BuildRecursively(newDropdownItem, childNode);
            }
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if(item is ValueReferenceAdvancedDropdownItem casted)
            {
                OnSelected?.Invoke(casted.Asset);
            }
        }

        private string GetItemName(string inputName)
        {
            if(inputName == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME)
                return "ORPHAN VALUES";

            return inputName;
        }

        private bool IsAllowedType(ValueReferenceAsset asset)
        {
            if(_filterType == null)
                return true;

            var interfaces = asset.GetType().GetInterfaces();

            foreach(var i in interfaces)
            {
                if(i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IValueReferenceSource<>) && 
                    i.GetGenericArguments().First() == _filterType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
