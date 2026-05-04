using Paps.Optionals;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<ILateUpdatable> GetDefaultLateUpdateUpdater()
        {
            return _lateUpdateUpdaters[0];
        }
        
        public void RegisterForLateUpdate(ILateUpdatable updatable, Optional<Updater<ILateUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultLateUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Register(GetDefaultLateUpdateUpdater(), updatable);

                return;
            }

            var updaterValue = updater.Value;

            if(ContainsUpdater(updaterValue))
            {
                if(updateSchemaGroupId.HasValue)
                    Register(updaterValue, updatable, updateSchemaGroupId);
                else
                    Register(updaterValue, updatable);
            }
            else
            {
                ThrowUpdaterNotFoundException(updaterValue);
            }
        }

        public void UnregisterFromLateUpdate(ILateUpdatable updatable, Optional<Updater<ILateUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultLateUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultLateUpdateUpdater(), updatable);

                return;
            }

            var updaterValue = updater.Value;

            if(ContainsUpdater(updaterValue))
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(updaterValue, updatable, updateSchemaGroupId);
                else
                    Unregister(updaterValue, updatable);
            }
            else
            {
                ThrowUpdaterNotFoundException(updaterValue);
            }
        }
    }
}
