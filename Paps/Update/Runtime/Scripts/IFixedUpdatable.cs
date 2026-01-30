namespace Paps.Update
{
    public  interface IFixedUpdatable : IUpdateMethodListener
    {
        public void ManagedFixedUpdate();
    }
}