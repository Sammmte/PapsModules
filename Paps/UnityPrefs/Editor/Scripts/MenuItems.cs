using System.IO;
using UnityEditor;

namespace Paps.UnityPrefs
{
    internal static class MenuItems
    {
        private const string BASE_PATH = "Paps/Unity Prefs/";

        [MenuItem(BASE_PATH + "ProjectPrefs/Clear")]
        private static void ClearProjectPrefs()
        {
            if(!Directory.Exists(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH))
                return;

            Directory.Delete(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH, true);
        }

        [MenuItem(BASE_PATH + "ProjectPrefs/Open Folder")]
        private static void OpenProjectPrefsFolder()
        {
            EnsureDirectoryExists(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH);

            EditorUtility.RevealInFinder(UnityPrefs.PROJECT_PREFS_BASE_FILE_PATH);
        }

        [MenuItem(BASE_PATH + "UserProjectPrefs/Clear")]
        private static void ClearUserProjectPrefs()
        {
            if(!Directory.Exists(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH))
                return;

            Directory.Delete(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH, true);
        }

        [MenuItem(BASE_PATH + "UserProjectPrefs/Open Folder")]
        private static void OpenUserProjectPrefsFolder()
        {
            EnsureDirectoryExists(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH);

            EditorUtility.RevealInFinder(UnityPrefs.USER_PROJECT_PREFS_BASE_FILE_PATH);
        }

        [MenuItem(BASE_PATH + "PlayerPrefsFileBased/Clear")]
        private static void ClearPlayerPrefsFileBased()
        {
            if(!Directory.Exists(UnityPrefs.PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH))
                return;

            Directory.Delete(UnityPrefs.PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH, true);
        }

        [MenuItem(BASE_PATH + "PlayerPrefsFileBased/Open Folder")]
        private static void OpenPlayerPrefsFileBasedFolder()
        {
            EnsureDirectoryExists(UnityPrefs.PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH);

            EditorUtility.RevealInFinder(UnityPrefs.PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH);
        }

        private static void EnsureDirectoryExists(string directory)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
