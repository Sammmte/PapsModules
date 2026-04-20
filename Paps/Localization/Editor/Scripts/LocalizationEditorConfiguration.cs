using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Localization.Editor
{
    public class LocalizationEditorConfiguration : ScriptableObject
    {
        private static LocalizationEditorConfiguration _instance;

        public static LocalizationEditorConfiguration Instance
        {
            get
            {
                if(_instance == null)
                {
                    var guids = AssetDatabase.FindAssets($"t:{nameof(LocalizationEditorConfiguration)}");
                    if (guids.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        _instance = AssetDatabase.LoadAssetAtPath<LocalizationEditorConfiguration>(path);
                    }
                }

                return _instance;
            }
        }

        [field: SerializeField] public VisualTreeAsset LocalizationFieldTreeAsset { get; private set; }
        [field: SerializeField] public Vector2 AdvancedDropdownMinimumSize { get; private set; } = new Vector2(300, 500);
    }
}
