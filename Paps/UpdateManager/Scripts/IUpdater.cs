namespace Paps.UpdateManager
{
    public interface IUpdater<T> where T : IUpdateMethodListener
    {
        public void Register(T listener);
        public void Unregister(T listener);
    }
}