using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferenceAdvancedDropdownItem : AdvancedDropdownItem
    {
        public ValueReferenceAsset Asset { get; }

        public ValueReferenceAdvancedDropdownItem(string name, ValueReferenceAsset asset) : base(name)
        {
            Asset = asset;
        }
    }
}
