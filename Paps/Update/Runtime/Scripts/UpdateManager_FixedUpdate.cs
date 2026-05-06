namespace Paps.Update
{
    public partial class UpdateManager
    {
        private Updater<IFixedUpdatable> GetDefaultFixedUpdateUpdater()
        {
            return _fixedUpdateUpdaters[0];
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable updatable, Updater<IFixedUpdatable> updater = null, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultFixedUpdateUpdater();

            if(ContainsUpdater(updater))
            {
                Register(updater, updatable, updateSchemaGroup);
            }
            else
            {
                ThrowUpdaterNotFoundException(updater);
            }
        }

        public void UnregisterFromFixedUpdate(IFixedUpdatable updatable, Updater<IFixedUpdatable> updater = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            if(updater == null)
                updater = GetDefaultFixedUpdateUpdater();

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
