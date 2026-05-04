namespace Paps.Update
{
    public sealed class DefaultLateUpdaterRunner : DefaultUpdaterRunner<ILateUpdatable>
    {
        private void LateUpdate()
        {
            UpdateSchema.Update();
        }
    }
}