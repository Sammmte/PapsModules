namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<ILateUpdatable> GetDefaultLateUpdateUpdater()
        {
            return _lateUpdateUpdaters[0];
        }
        
        public void RegisterForLateUpdate(ILateUpdatable updatable, Updater<ILateUpdatable> updater = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultLateUpdateUpdater();

            if(ContainsUpdater(updater))
            {
                Register(updater, updatable, updateSchemaGroup);
            }
            else
            {
                ThrowUpdaterNotFoundException(updater);
            }
        }

        public void UnregisterFromLateUpdate(ILateUpdatable updatable, Updater<ILateUpdatable> updater = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultLateUpdateUpdater();

            if(ContainsUpdater(updater))
            {
                Unregister(updater, updatable, updateSchemaGroup);
            }
            else
            {
                ThrowUpdaterNotFoundException(updater);
            }
        }
    }
}
