using System.Linq;
using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = ValueReferenceAsset.BASE_CREATE_ASSET_MENU_PATH + "Group")]
    public class ValueReferenceGroupAsset : ScriptableObject
    {
        [field: SerializeField] public string GroupPath { get; private set; }
        [SerializeField] private ValueReferenceAsset[] _valueReferences;

        public ValueReferenceAsset<T>[] GetReferencesOfType<T>()
        {
            return _valueReferences.OfType<ValueReferenceAsset<T>>().ToArray();
        }
    }
}