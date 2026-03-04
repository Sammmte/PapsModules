using UnityEngine.Pool;

namespace Paps.Metadata
{

    internal abstract class Metadata
    {
        public abstract void Release();
    }

    internal sealed class Metadata<T> : Metadata
    {
        private static ObjectPool<Metadata<T>> _pool;

        public static Metadata<T> GetPooled(T data, int keysAmount)
        {
            if(_pool == null)
            {
                _pool = new ObjectPool<Metadata<T>>(static () => new Metadata<T>(), defaultCapacity: keysAmount);
            }

            var metadata = _pool.Get();

            metadata.Value = data;

            return metadata;
        }

        public T Value { get; internal set; }

        private Metadata() { }

        public override void Release()
        {
            Value = default;
            _pool.Release(this);
        }
    }
}
