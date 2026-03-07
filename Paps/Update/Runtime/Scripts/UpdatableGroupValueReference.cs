using Paps.ValueReferences;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Update/Updatable Group Reference")]
    public class UpdatableGroupValueReference : ValueReferenceAsset<int>
    {
        [SerializeField] private UpdatableGroup _group;

        protected override int GetValue()
        {
            return UpdateSchemaUtils.GetIdOfGroup(_group);
        }
    }
}