using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Paps.Localization.Editor
{
    public class LocalizationIdAdvancedDropdown : AdvancedDropdown
    {
        private class LocalizationIdItem : AdvancedDropdownItem
        {
            public string TableId { get; private set; }
            public string LocalizationId { get; private set; }

            public LocalizationIdItem(string name, string tableId, string localizationId) : base(name)
            {
                TableId = tableId;
                LocalizationId = localizationId;
            }
        }

        private Dictionary<string, string[]> _localizationIdsPerTable;

        public event Action<LocalizationIdReference> OnItemSelected;

        public LocalizationIdAdvancedDropdown(AdvancedDropdownState state, Dictionary<string, string[]> localizationIdsPerTable) : base(state)
        {
            _localizationIdsPerTable = localizationIdsPerTable;

            minimumSize = new Vector2(300, 500);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Localization Ids");

            foreach(var tableWithKeys in _localizationIdsPerTable)
            {
                var tableItem = new AdvancedDropdownItem(tableWithKeys.Key);
                root.AddChild(tableItem);

                foreach(var localizationId in tableWithKeys.Value)
                {
                    tableItem.AddChild(new LocalizationIdItem(localizationId, tableWithKeys.Key, localizationId));
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if(item is LocalizationIdItem localizationItem)
            {
                OnItemSelected?.Invoke(new LocalizationIdReference
                {
                    TableId = localizationItem.TableId,
                    LocalizationId = localizationItem.LocalizationId
                });
            }
        }
    }
}
