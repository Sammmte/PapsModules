using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Float")]
    public class FloatValueReferenceAsset : ValueReferenceAsset<float>
    {
        [SerializeField] private float _value;

        protected override float GetValue() => _value;
        protected override void SetValue(float value) => _value = value;
    }
}