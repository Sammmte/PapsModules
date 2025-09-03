using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Boolean")]
    public class BooleanValueReferenceAsset : ValueReferenceAsset<bool>
    {
        [SerializeField] private bool _value;

        protected override bool GetValue() => _value;
    }
}