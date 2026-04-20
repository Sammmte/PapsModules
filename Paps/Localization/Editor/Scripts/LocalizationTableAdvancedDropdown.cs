using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Paps.Localization.Editor
{
    public class LocalizationTableAdvancedDropdown : AdvancedDropdown
    {
        private string[] _tableIds;

        public event Action<string> OnItemSelected;

        public LocalizationTableAdvancedDropdown(AdvancedDropdownState state, string[] tableIds) : base(state)
        {
            _tableIds = tableIds;

            minimumSize = LocalizationEditorConfiguration.Instance.AdvancedDropdownMinimumSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Localization Tables");

            foreach(var tableId in _tableIds)
            {
                root.AddChild(new AdvancedDropdownItem(tableId));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            OnItemSelected?.Invoke(item.name);
        }
    }
}
