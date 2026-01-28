namespace Paps.Update
{
    public interface IUpdatable : IUpdateMethodListener
    {
        public void ManagedUpdate();
    }
}