using SaintsField;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Paps.ValueReferences
{
    [Serializable]
    public struct ValueReference<T>
    {
        [SerializeField] [Dropdown(nameof(GetOptions))] private ValueReferenceAsset<T> _referenceAsset;

        public T Value
        {
            get => _referenceAsset.Value;
            set => _referenceAsset.Value = value;
        }

        public static implicit operator T(ValueReference<T> valueReference) => valueReference.Value;

        private static DropdownList<ValueReferenceAsset<T>> GetOptions()
        {
            var dropdownItems = AssetDatabase.FindAssets($"t:{nameof(ValueReferenceGroupAsset)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ValueReferenceGroupAsset>(AssetDatabase.GUIDToAssetPath((string)guid)))
                .Select(group => (Path: group.GroupPath, References: group.GetReferencesOfType<T>()))
                .Where(pathWithReferences => pathWithReferences.References.Length != 0)
                .SelectMany(pathWithReferences => pathWithReferences.References
                    .Select(reference => (Path: pathWithReferences.Path + $"/{reference.name}", Reference: reference)));

            return new DropdownList<ValueReferenceAsset<T>>(dropdownItems);
        }
    }
}