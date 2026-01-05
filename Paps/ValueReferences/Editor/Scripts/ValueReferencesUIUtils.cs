using UnityEditor.IMGUI.Controls;

namespace Paps.ValueReferences.Editor
{
    public static class ValueReferencesUIUtils
    {
        public static ValueReferencesAdvancedDropdown CreateAdvancedDropdown()
        {
            return new ValueReferencesAdvancedDropdown(new AdvancedDropdownState(), 
                ValueReferencesEditorManager.GetGroupsPathTree());
        }
    }
}
