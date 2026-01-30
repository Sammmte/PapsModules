namespace Paps.Update
{
    public interface ILateUpdatable : IUpdateMethodListener
    {
        public void ManagedLateUpdate();
    }
}