using Paps.ValueReferences;
using SaintsField;
using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = BASE_CREATE_ASSET_MENU_PATH + "Update/Updatable Group Reference")]
    public class UpdatableGroupValueReference : ValueReferenceAsset<int>
    {
        [SerializeField] private string _group;
        [SerializeField, ReadOnly] private int _groupId;

        private void OnValidate()
        {
            _groupId = UpdateSchemaUtils.GetIdOfGroup(_group);
        }

        protected override int GetValue()
        {
            return _groupId;
        }
    }
}