using System;
using System.Collections.Generic;

namespace Paps.UnityExtensions
{
    public struct TempReadOnlyBufferSegment<T> : IDisposable
    {
        private T[] _array;
        private IList<T> _list;
        public int Length { get; }

        public T this[int index]
        {
            get
            {
                if (_list != null)
                    return _list[index];

                return _array[index];
            }
        }
        
        public TempReadOnlyBufferSegment(T[] array, int length)
        {
            _array = array;
            _list = null;
            Length = length;
        }

        public TempReadOnlyBufferSegment(IList<T> list, int length)
        {
            _list = list;
            _array = null;
            Length = length;
        }
        
        public void Dispose()
        {
            if (_list != null)
            {
                _list.Clear();
            }
            else
            {
                ClearArray();
            }
        }

        private void ClearArray()
        {
            for (int i = 0; i < Length; i++)
            {
                _array[i] = default;
            }
        }
    }
}