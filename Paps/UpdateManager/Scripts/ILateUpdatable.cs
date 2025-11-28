namespace Paps.UpdateManager
{
    public interface ILateUpdatable : IUpdateMethodListener
    {
        public void ManagedLateUpdate();
    }
}