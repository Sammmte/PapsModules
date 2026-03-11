using SaintsField;
using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Layer")]
    public class LayerValueReferenceAsset : ValueReferenceAsset<int>
    {
        [SerializeField, Layer] private int _value;

        protected override int GetValue() => _value;
    }
}