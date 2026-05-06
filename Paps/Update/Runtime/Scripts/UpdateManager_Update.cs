namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<IUpdatable> GetDefaultUpdateUpdater()
        {
            return _updateUpdaters[0];
        }

        public void RegisterForUpdate(IUpdatable updatable, Updater<IUpdatable> updater = null, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultUpdateUpdater();

            if(ContainsUpdater(updater))
            {
                Register(updater, updatable, updateSchemaGroup);
            }
            else
            {
                ThrowUpdaterNotFoundException(updater);
            }
        }

        public void UnregisterFromUpdate(IUpdatable updatable, Updater<IUpdatable> updater = null, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultUpdateUpdater();

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
