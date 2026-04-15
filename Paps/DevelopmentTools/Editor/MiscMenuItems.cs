using UnityEditor;

namespace Paps.DevelopmentTools.Editor
{
    public static class MiscMenuItems
    {
        [MenuItem("Paps/EditorPrefs/Delete EditorPrefs")]
        public static void DeleteEditorPrefs()
        {
            if (EditorUtility.DisplayDialog("Delete EditorPrefs", "Are you sure you want to delete all EditorPrefs?", "Yes", "No"))
            {
                EditorPrefs.DeleteAll();
            }
        }
    }
}