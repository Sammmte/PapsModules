namespace Paps.Update
{
    public sealed class DefaultUpdateUpdaterRunner : DefaultUpdaterRunner<IUpdatable>
    {
        private void Update()
        {
            UpdateSchema.Update();
        }
    }
}