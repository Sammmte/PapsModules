namespace Paps.Update
{
    public static class UpdateMethodListenerExtensions
    {
        public static void RegisterUpdate(this IUpdatable updatable, Updater<IUpdatable> updaterId = null, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.RegisterForUpdate(updatable, updaterId, updateSchemaGroup);
        }

        public static void UnregisterUpdate(this IUpdatable updatable, Updater<IUpdatable> updaterId = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.UnregisterFromUpdate(updatable, updaterId, updateSchemaGroup);
        }
        
        public static void RegisterLateUpdate(this ILateUpdatable lateUpdatable, Updater<ILateUpdatable> updaterId = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.RegisterForLateUpdate(lateUpdatable, updaterId, updateSchemaGroup);
        }
        
        public static void UnregisterLateUpdate(this ILateUpdatable lateUpdatable, Updater<ILateUpdatable> updaterId = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.UnregisterFromLateUpdate(lateUpdatable, updaterId, updateSchemaGroup);
        }

        public static void RegisterFixedUpdate(this IFixedUpdatable fixedUpdatable, Updater<IFixedUpdatable> updaterId = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.RegisterForFixedUpdate(fixedUpdatable, updaterId, updateSchemaGroup);
        }
        
        public static void UnregisterFixedUpdate(this IFixedUpdatable fixedUpdatable, Updater<IFixedUpdatable> updaterId = default, 
            UpdateSchemaGroup updateSchemaGroup = null)
        {
            UpdateManager.Instance.UnregisterFromFixedUpdate(fixedUpdatable, updaterId, updateSchemaGroup);
        }
    }
}