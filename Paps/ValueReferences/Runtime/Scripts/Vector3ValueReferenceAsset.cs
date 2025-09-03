using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Vector3")]
    public class Vector3ValueReferenceAsset : ValueReferenceAsset<Vector3>
    {
        [SerializeField] private Vector3 _value;

        protected override Vector3 GetValue() => _value;
    }
}