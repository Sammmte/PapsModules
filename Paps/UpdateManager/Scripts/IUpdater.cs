namespace Paps.UpdateManager
{
    public interface IUpdater<T> where T : IUpdateMethodListener
    {
        public int Id { get; }
        public void Register(T listener);
        public void Unregister(T listener);
        public void Enable();
        public void Disable();
    }
}