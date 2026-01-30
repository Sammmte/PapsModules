using Paps.Optionals;

namespace Paps.Update
{
    public static class UpdateMethodListenerExtensions
    {
        public static void RegisterUpdate(this IUpdatable updatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.RegisterForUpdate(updatable, updaterId, updateSchemaGroupId);
        }

        public static void UnregisterUpdate(this IUpdatable updatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.UnregisterFromUpdate(updatable, updaterId, updateSchemaGroupId);
        }
        
        public static void RegisterLateUpdate(this ILateUpdatable lateUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.RegisterForLateUpdate(lateUpdatable, updaterId, updateSchemaGroupId);
        }
        
        public static void UnregisterLateUpdate(this ILateUpdatable lateUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.UnregisterFromLateUpdate(lateUpdatable, updaterId, updateSchemaGroupId);
        }

        public static void RegisterFixedUpdate(this IFixedUpdatable fixedUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.RegisterForFixedUpdate(fixedUpdatable, updaterId, updateSchemaGroupId);
        }
        
        public static void UnregisterFixedUpdate(this IFixedUpdatable fixedUpdatable, Optional<int> updaterId = default, 
            Optional<int> updateSchemaGroupId = default)
        {
            UpdateManager.Instance.UnregisterFromFixedUpdate(fixedUpdatable, updaterId, updateSchemaGroupId);
        }
    }
}