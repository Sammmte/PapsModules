using System.IO;
using UnityEditor;

namespace Paps.UnityPrefs
{
    internal static class MenuItems
    {
        private const string BASE_PATH = "Paps/Unity Prefs/";

        [MenuItem(BASE_PATH + "Clear ProjectPrefs")]
        private static void ClearProjectPrefs()
        {
            if(!Directory.Exists(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH))
                return;

            Directory.Delete(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH, true);
        }

        [MenuItem(BASE_PATH + "Open ProjectPrefs Folder")]
        private static void OpenProjectPrefsFolder()
        {
            EditorUtility.RevealInFinder(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH);
        }

        [MenuItem(BASE_PATH + "Clear UserProjectPrefs")]
        private static void ClearUserProjectPrefs()
        {
            if(!Directory.Exists(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH))
                return;

            Directory.Delete(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH, true);
        }

        [MenuItem(BASE_PATH + "Open UserProjectPrefs Folder")]
        private static void OpenUserProjectPrefsFolder()
        {
            EditorUtility.RevealInFinder(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH);
        }
    }
}
