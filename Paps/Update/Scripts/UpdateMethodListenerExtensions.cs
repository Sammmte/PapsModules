namespace Paps.Update
{
    public static class UpdateMethodListenerExtensions
    {
        public static void RegisterUpdate(this IUpdatable updatable)
        {
            UpdateManager.Instance.RegisterForUpdate(updatable);
        }
        
        public static void RegisterUpdate(this IUpdatable updatable, int updaterId)
        {
            UpdateManager.Instance.RegisterForUpdate(updatable, updaterId);
        }
        
        public static void UnregisterUpdate(this IUpdatable updatable)
        {
            UpdateManager.Instance.UnregisterFromUpdate(updatable);
        }

        public static void UnregisterUpdate(this IUpdatable updatable, int updaterId)
        {
            UpdateManager.Instance.UnregisterFromUpdate(updatable, updaterId);
        }
        
        public static void RegisterLateUpdate(this ILateUpdatable lateUpdatable)
        {
            UpdateManager.Instance.RegisterForLateUpdate(lateUpdatable);
        }
        
        public static void RegisterLateUpdate(this ILateUpdatable lateUpdatable, int updaterId)
        {
            UpdateManager.Instance.RegisterForLateUpdate(lateUpdatable, updaterId);
        }
        
        public static void UnregisterLateUpdate(this ILateUpdatable lateUpdatable)
        {
            UpdateManager.Instance.UnregisterFromLateUpdate(lateUpdatable);
        }
        
        public static void UnregisterLateUpdate(this ILateUpdatable lateUpdatable, int updaterId)
        {
            UpdateManager.Instance.UnregisterFromLateUpdate(lateUpdatable, updaterId);
        }
        
        public static void RegisterFixedUpdate(this IFixedUpdatable fixedUpdatable)
        {
            UpdateManager.Instance.RegisterForFixedUpdate(fixedUpdatable);
        }

        public static void RegisterFixedUpdate(this IFixedUpdatable fixedUpdatable, int updaterId)
        {
            UpdateManager.Instance.RegisterForFixedUpdate(fixedUpdatable, updaterId);
        }
        
        public static void UnregisterFixedUpdate(this IFixedUpdatable fixedUpdatable)
        {
            UpdateManager.Instance.UnregisterFromFixedUpdate(fixedUpdatable);
        }
        
        public static void UnregisterFixedUpdate(this IFixedUpdatable fixedUpdatable, int updaterId)
        {
            UpdateManager.Instance.UnregisterFromFixedUpdate(fixedUpdatable, updaterId);
        }
    }
}