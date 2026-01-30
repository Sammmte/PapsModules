using Paps.Optionals;
using System;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private IUpdater<IUpdatable> GetDefaultUpdateUpdater()
        {
            return _updateUpdaters[0].Updater;
        }

        public IUpdater<IUpdatable> GetUpdateUpdaterById(int id)
        {
            if(_updateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {id}");
            }
        }

        public void RegisterForUpdate(IUpdatable updatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Register(GetDefaultUpdateUpdater(), updatable);

                return;
            }

            if(_updateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Register(updater, updatable, updateSchemaGroupId);
                else
                    Register(updater, updatable);
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {updaterId}");
            }
        }

        public void UnregisterFromUpdate(IUpdatable updatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updaterId.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultUpdateUpdater(), updatable);

                return;
            }

            if(_updateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(updater, updatable, updateSchemaGroupId);
                else
                    Unregister(updater, updatable);
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {updaterId}");
            }
        }
    }
}
