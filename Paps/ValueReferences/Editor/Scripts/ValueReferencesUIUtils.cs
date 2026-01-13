using Paps.UnityPrefs;
using UnityEditor.IMGUI.Controls;

namespace Paps.ValueReferences.Editor
{
    public static class ValueReferencesUIUtils
    {
        private const string PREFS_SCOPE = "value-references-editor";

        public static readonly UnityPref VALUE_REFERENCES_EDITOR_USER_PROJECT_PREFS = UnityPrefs.UnityPrefs.GetPref(UnityPrefType.UserProjectPrefs, PREFS_SCOPE);

        public static ValueReferencesAdvancedDropdown CreateAdvancedDropdown()
        {
            return new ValueReferencesAdvancedDropdown(new AdvancedDropdownState(), 
                ValueReferencesEditorManager.GetGroupsPathTree());
        }

        public static ValueReferencesAdvancedDropdown CreateAdvancedDropdownForType<T>()
        {
            return new ValueReferencesAdvancedDropdown(new AdvancedDropdownState(), 
                ValueReferencesEditorManager.GetGroupsPathTreeContainingType<T>(), typeof(T));
        }
    }
}
