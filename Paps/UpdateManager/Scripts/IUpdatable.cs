namespace Paps.UpdateManager
{
    public interface IUpdatable : IUpdateMethodListener
    {
        public void ManagedUpdate();
    }
}