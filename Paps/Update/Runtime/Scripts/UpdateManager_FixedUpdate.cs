using Paps.Optionals;
using System;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private IUpdater<IFixedUpdatable> GetDefaultFixedUpdateUpdater()
        {
            return _fixedUpdateUpdaters[0].Updater;
        }

        public IUpdater<IFixedUpdatable> GetFixedUpdateUpdaterById(int id)
        {
            if(_fixedUpdateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {id}");
            }
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable fixedUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultFixedUpdateUpdater(), fixedUpdatable, updateSchemaGroupId);
                else
                    Register(GetDefaultFixedUpdateUpdater(), fixedUpdatable);

                return;
            }

            if(_fixedUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Register(updater, fixedUpdatable, updateSchemaGroupId);
                else
                    Register(updater, fixedUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {updaterId}");
            }
        }
        
        public void UnregisterFromFixedUpdate(IFixedUpdatable fixedUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultFixedUpdateUpdater(), fixedUpdatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultFixedUpdateUpdater(), fixedUpdatable);

                return;
            }

            if(_fixedUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(updater, fixedUpdatable, updateSchemaGroupId);
                else
                    Unregister(updater, fixedUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {updaterId}");
            }
        }
    }
}
