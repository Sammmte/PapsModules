using Paps.ValueReferences;
using UnityEngine;

namespace Paps.Physics
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Force Mode")]
    public class ForceModeValueReferenceAsset : ValueReferenceAsset<ForceMode>
    {
        [SerializeField] private ForceMode _value;

        protected override ForceMode GetValue() => _value;
    }
}