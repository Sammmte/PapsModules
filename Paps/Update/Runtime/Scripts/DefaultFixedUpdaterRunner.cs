namespace Paps.Update
{
    public sealed class DefaultFixedUpdaterRunner : DefaultUpdaterRunner<IFixedUpdatable>
    {
        private void FixedUpdate()
        {
            UpdateSchema.Update();
        }
    }
}