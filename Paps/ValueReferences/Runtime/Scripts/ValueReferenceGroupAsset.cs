using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paps.ValueReferences
{
    [CreateAssetMenu(menuName = ValueReferenceAsset.BASE_CREATE_ASSET_MENU_PATH + "Group")]
    public class ValueReferenceGroupAsset : ScriptableObject
    {
        [field: SerializeField] public string Path { get; set; }
        [SerializeField] public ValueReferenceAsset[] ValueReferenceAssets;
    }
}