using Paps.Optionals;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<IUpdatable> GetDefaultUpdateUpdater()
        {
            return _updateUpdaters[0];
        }

        public void RegisterForUpdate(IUpdatable updatable, Optional<Updater<IUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Register(GetDefaultUpdateUpdater(), updatable);

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

        public void UnregisterFromUpdate(IUpdatable updatable, Optional<Updater<IUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultUpdateUpdater(), updatable);

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
