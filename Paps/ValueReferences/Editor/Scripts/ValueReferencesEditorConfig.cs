using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferencesEditorConfig : ScriptableObject
    {
        private static ValueReferencesEditorConfig _instance;

        public static ValueReferencesEditorConfig Instance
        {
            get
            {
                if(_instance == null)
                {
                    var guid = AssetDatabase.FindAssets($"t:{nameof(ValueReferencesEditorConfig)}").First();

                    _instance = AssetDatabase.LoadAssetAtPath<ValueReferencesEditorConfig>(AssetDatabase.GUIDToAssetPath(guid));
                }

                return _instance;
            }
        }

        [field: SerializeField] public VisualTreeAsset ValueReferencePropertyDrawerVTA { get; private set; }
    }
}
