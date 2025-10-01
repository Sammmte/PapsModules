namespace Paps.Metadata
{
    public interface IMetadata<out TValue>
    {
        public TValue Value { get; }
    }
}