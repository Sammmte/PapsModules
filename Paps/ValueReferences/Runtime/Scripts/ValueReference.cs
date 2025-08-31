using SaintsField;
using SaintsField.Playa;
using System;
using System.Linq;
using UnityEngine;

namespace Paps.ValueReferences
{
    [Serializable]
    public struct ValueReference<T>
    {
        [SerializeField] [Dropdown(nameof(GetOptions))] private ValueReferenceAsset<T> _referenceAsset;

        [ShowInInspector]
        public T Value
        {
            get
            {
                if (_referenceAsset == null)
                    return default;
                
                return _referenceAsset.Value;
            }
            set => _referenceAsset.Value = value;
        }

        #if UNITY_EDITOR
        [Button]
        private void PingAsset()
        {
            UnityEditor.EditorGUIUtility.PingObject(_referenceAsset);
        }

        public static implicit operator T(ValueReference<T> valueReference) => valueReference.Value;

        private static DropdownList<ValueReferenceAsset<T>> GetOptions()
        {
            var dropdownItems = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ValueReferenceGroupAsset)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<ValueReferenceGroupAsset>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .Select(group => group.GetReferencesOfType<T>())
                .Where(references => references.Length != 0)
                .SelectMany(references => references.Select(r => (r.Path, r.ValueReferenceAsset)));

            return new DropdownList<ValueReferenceAsset<T>>(dropdownItems);
        }
        #endif
    }
}