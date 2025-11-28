namespace Paps.UpdateManager
{
    public static class UpdateMethodListenerExtensions
    {
        public static void RegisterUpdate(this IUpdatable updatable)
        {
            UpdateManager.Instance.RegisterForUpdate(updatable);
        }
        
        public static void UnregisterUpdate(this IUpdatable updatable)
        {
            UpdateManager.Instance.UnregisterFromUpdate(updatable);
        }
        
        public static void RegisterLateUpdate(this ILateUpdatable lateUpdatable)
        {
            UpdateManager.Instance.RegisterForLateUpdate(lateUpdatable);
        }
        
        public static void UnregisterLateUpdate(this ILateUpdatable lateUpdatable)
        {
            UpdateManager.Instance.UnregisterFromLateUpdate(lateUpdatable);
        }
        
        public static void RegisterFixedUpdate(this IFixedUpdatable fixedUpdatable)
        {
            UpdateManager.Instance.RegisterForFixedUpdate(fixedUpdatable);
        }
        
        public static void UnregisterFixedUpdate(this IFixedUpdatable fixedUpdatable)
        {
            UpdateManager.Instance.UnregisterFromFixedUpdate(fixedUpdatable);
        }
    }
}