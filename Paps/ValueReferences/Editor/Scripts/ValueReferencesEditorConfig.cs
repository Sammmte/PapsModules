using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    public class ValueReferencesEditorConfig : ScriptableObject
    {
        public static ValueReferencesEditorConfig Instance { get; private set; }

        [field: SerializeField] public VisualTreeAsset ValueReferencePropertyDrawerVTA { get; private set; }
        [SerializeField] private ValueReference<int> algo;

        private void OnEnable()
        {
            Instance = this;
        }
    }
}
