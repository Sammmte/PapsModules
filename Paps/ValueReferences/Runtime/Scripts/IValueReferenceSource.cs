namespace Paps.ValueReferences
{
    public interface IValueReferenceSource { }

    public interface IValueReferenceSource<T> : IValueReferenceSource
    {
        public T Value { get; }
    }
}
