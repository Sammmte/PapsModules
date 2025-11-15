using Paps.ValueReferences;
using UnityEngine;

namespace Paps.Curves
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Curve")]
    public class CurveValueReferenceAsset : ValueReferenceAsset<Curve>
    {
        [SerializeField] private Curve _value;

        protected override Curve GetValue() => _value;
    }
}