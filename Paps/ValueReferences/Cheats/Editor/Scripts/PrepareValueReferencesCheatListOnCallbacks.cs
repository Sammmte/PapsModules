using Paps.Build;
using Paps.ValueReferences.Editor;
using System.Linq;
using UnityEditor;

namespace Paps.ValueReferences.Cheats.Editor
{
    public class PrepareValueReferencesCheatListOnCallbacks : IBuildPreprocessor
    {
        [InitializeOnLoadMethod]
        public static void ListenCallbacks()
        {
            EditorApplication.playModeStateChanged += PrepareValueReferencesForPlayMode;
        }

        private static void PrepareValueReferencesForPlayMode(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.ExitingEditMode)
                UpdateValueReferencesCheatListAsset();
        }

        private static void UpdateValueReferencesCheatListAsset()
        {
            var guid = AssetDatabase.FindAssets($"t:{nameof(ValueReferencesCheatList)}").FirstOrDefault();
            if (guid == null)
                return;

            var listAsset = AssetDatabase.LoadAssetAtPath<ValueReferencesCheatList>(AssetDatabase.GUIDToAssetPath(guid));

            var groupAssets = ValueReferencesEditorManager.GetGroupAssets();

            listAsset.RawGroupAssets = groupAssets.Where(g => g.Path != ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME).ToArray();
            listAsset.RawOrphanAssets = groupAssets.First(g => g.Path == ValueReferencesEditorManager.ORPHAN_GROUP_PATH_NAME).ValueReferenceAssets;
            
            EditorUtility.SetDirty(listAsset);
            AssetDatabase.SaveAssets();
        }

        public void Process(BuildSettings currentBuildSettings)
        {
            UpdateValueReferencesCheatListAsset();
        }
    }
}
