using System;

namespace Paps.Update
{
    public interface IUpdater<T> : IDisposable where T : IUpdateMethodListener
    {
        public void Register(T listener);
        public void Register(T listener, int updateSchemaGroupId);
        public void Unregister(T listener);
        public void Unregister(T listener, int updateSchemaGroupId);
        public void Enable();
        public void Disable();
    }
}