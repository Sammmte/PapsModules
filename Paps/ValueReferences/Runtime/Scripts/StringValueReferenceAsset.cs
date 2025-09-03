using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "String")]
    public class StringValueReferenceAsset : ValueReferenceAsset<string>
    {
        [SerializeField] private string _value;

        protected override string GetValue() => _value;
    }
}