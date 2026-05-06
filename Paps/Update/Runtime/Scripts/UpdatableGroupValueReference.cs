using Paps.ValueReferences;
using SaintsField;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Update/Updatable Group Reference")]
    public class UpdatableGroupValueReference : ValueReferenceAsset<UpdateSchemaGroup>
    {
        [SerializeField] private UpdateSchemaGroup _group;

        protected override UpdateSchemaGroup GetValue()
        {
            return _group;
        }
    }
}