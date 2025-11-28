namespace Paps.UpdateManager
{
    public  interface IFixedUpdatable : IUpdateMethodListener
    {
        public void ManagedFixedUpdate();
    }
}