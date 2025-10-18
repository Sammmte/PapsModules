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
        [SerializeField] 
        #if UNITY_EDITOR
        [AdvancedDropdown(nameof(GetOptions))]
        #endif
        private ValueReferenceAsset<T> _referenceAsset;

        [ShowInInspector]
        public T Value
        {
            get
            {
                if (_referenceAsset == null)
                    return default;
                
                return _referenceAsset.Value;
            }
        }
        
        public static implicit operator T(ValueReference<T> valueReference) => valueReference.Value;

        #if UNITY_EDITOR
        public const string UNCATEGORIZED_GROUP = "Uncategorized";
        
        [Button]
        private void PingAsset()
        {
            UnityEditor.EditorGUIUtility.PingObject(_referenceAsset);
        }
        
        private static AdvancedDropdownList<ValueReferenceAsset<T>> GetOptions()
        {
            var list = new AdvancedDropdownList<ValueReferenceAsset<T>>("Reference Asset");
            
            var dropdownItems = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ValueReferenceGroupAsset)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<ValueReferenceGroupAsset>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .Select(group => group.GetReferencesOfType<T>())
                .Where(references => references.Length != 0)
                .SelectMany(references => references.Select(r => (r.Path, r.ValueReferenceAsset)))
                .ToList();

            var uncategorized = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ValueReferenceAsset)}")
                .Select(guid =>
                    UnityEditor.AssetDatabase.LoadAssetAtPath<ValueReferenceAsset>(
                        UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .OfType<ValueReferenceAsset<T>>()
                .Where(asset => !dropdownItems.Any(tuple => tuple.ValueReferenceAsset == asset))
                .Select(asset => ($"{UNCATEGORIZED_GROUP}/{asset.name}", asset));
            
            dropdownItems.InsertRange(0, uncategorized);

            foreach (var valueReference in dropdownItems)
            {
                list.Add(valueReference.Path, valueReference.ValueReferenceAsset);
            }

            return list;
        }
        #endif
    }
}