using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Vector2")]
    public class Vector2ValueReferenceAsset : ValueReferenceAsset<Vector2>
    {
        [SerializeField] private Vector2 _value;

        protected override Vector2 GetValue() => _value;
    }
}