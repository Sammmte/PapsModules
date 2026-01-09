using Paps.ValueReferences;
using UnityEngine;

namespace Paps.Physics
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "LayerMask")]
    public class LayerMaskValueReferenceAsset : ValueReferenceAsset<LayerMask>
    {
        [SerializeField] private LayerMask _value;

        protected override LayerMask GetValue() => _value;
    }
}