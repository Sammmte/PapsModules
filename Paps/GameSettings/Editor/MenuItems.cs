using UnityEditor;
using System.Linq;
using System.IO;

namespace Paps.GameSettings.Editor
{
    public static class MenuItems
    {
        [MenuItem("Paps/Game Settings/Open File Folder")]
        public static void OpenFileFolder()
        {
            var guid = AssetDatabase.FindAssetGUIDs($"t:{nameof(FileGameSettingsStorage)}").First();

            var storage = AssetDatabase.LoadAssetByGUID<FileGameSettingsStorage>(guid);

            if(storage != null)
            {
                var file = storage.GetFilePath();

                if(File.Exists(file))
                {
                    EditorUtility.RevealInFinder(file);
                }
            }
        }

        [MenuItem("Paps/Game Settings/Delete File")]
        public static void DeleteFile()
        {
            var guid = AssetDatabase.FindAssetGUIDs($"t:{nameof(FileGameSettingsStorage)}").First();

            var storage = AssetDatabase.LoadAssetByGUID<FileGameSettingsStorage>(guid);

            if(storage != null)
            {
                var file = storage.GetFilePath();

                if(File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
