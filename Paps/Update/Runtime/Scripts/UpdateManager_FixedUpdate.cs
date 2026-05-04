using Paps.Optionals;

namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<IFixedUpdatable> GetDefaultFixedUpdateUpdater()
        {
            return _fixedUpdateUpdaters[0];
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable updatable, Optional<Updater<IFixedUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Register(GetDefaultFixedUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Register(GetDefaultFixedUpdateUpdater(), updatable);

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

        public void UnregisterFromFixedUpdate(IFixedUpdatable updatable, Optional<Updater<IFixedUpdatable>> updater = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            if(!updater.HasValue)
            {
                if(updateSchemaGroupId.HasValue)
                    Unregister(GetDefaultFixedUpdateUpdater(), updatable, updateSchemaGroupId);
                else
                    Unregister(GetDefaultFixedUpdateUpdater(), updatable);

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
