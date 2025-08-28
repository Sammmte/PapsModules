using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Int")]
    public class IntValueReferenceAsset : ValueReferenceAsset<int>
    {
        [SerializeField] private int _value;

        protected override int GetValue() => _value;
        protected override void SetValue(int value) => _value = value;
    }
}