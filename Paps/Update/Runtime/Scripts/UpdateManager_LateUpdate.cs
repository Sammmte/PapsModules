using Paps.Optionals;
using System;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private IUpdater<ILateUpdatable> GetDefaultLateUpdateUpdater()
        {
            return _lateUpdateUpdaters[0].Updater;
        }

        public IUpdater<ILateUpdatable> GetLateUpdateUpdaterById(int id)
        {
            if(_lateUpdateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {id}");
            }
        }
        
        public void RegisterForLateUpdate(ILateUpdatable lateUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultLateUpdateUpdater(), lateUpdatable, updateSchemaGroupId);
                else
                    Register(GetDefaultLateUpdateUpdater(), lateUpdatable);

                return;
            }

            if(_lateUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Register(updater, lateUpdatable, updateSchemaGroupId);
                else
                    Register(updater, lateUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {updaterId}");
            }
        }
        
        public void UnregisterFromLateUpdate(ILateUpdatable lateUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultLateUpdateUpdater(), lateUpdatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultLateUpdateUpdater(), lateUpdatable);

                return;
            }

            if(_lateUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(updater, lateUpdatable, updateSchemaGroupId);
                else
                    Unregister(updater, lateUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {updaterId}");
            }
        }
    }
}
