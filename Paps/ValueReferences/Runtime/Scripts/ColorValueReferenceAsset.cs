using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Color")]
    public class ColorValueReferenceAsset : ValueReferenceAsset<Color>
    {
        [SerializeField] private Color _value;

        protected override Color GetValue() => _value;
    }
}