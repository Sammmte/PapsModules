using Paps.ValueReferences;
using UnityEngine;

namespace Paps.Physics
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Collision Detection Mode")]
    public class CollisionDetectionModeValueReferenceAsset : ValueReferenceAsset<CollisionDetectionMode>
    {
        [SerializeField] private CollisionDetectionMode _value;

        protected override CollisionDetectionMode GetValue() => _value;
    }
}