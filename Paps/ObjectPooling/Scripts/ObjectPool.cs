using Paps.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Paps.ObjectPooling
{
    public class ObjectPool<T> : IDisposable where T : class
    {
        private List<T> _list;
        private Func<T> _createFunction;

        public Func<T> CreateFunction
        {
            get => _createFunction;
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("CreateFunction_Value");
                }

                _createFunction = value;
            }
        }

        public Action<T> ActionOnGet { get; set; }
        public Action<T> ActionOnRelease { get; set; }
        public Action<T> ActionOnDestroy { get; set; }

        public int CountAvailable => _list.Count;

        public int Capacity
        {
            get => _list.Capacity;
            set => _list.Capacity = value;
        }

        public ObjectPool(Func<T> createFunction, Action<T> actionOnGet = null, 
            Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null,
            int capacity = 10, bool prewarm = false)
        {
            if (createFunction == null)
            {
                throw new ArgumentNullException(nameof(createFunction));
            }

            _list = new List<T>(capacity);
            CreateFunction = createFunction;
            ActionOnGet = actionOnGet;
            ActionOnRelease = actionOnRelease;
            ActionOnDestroy = actionOnDestroy;

            if(prewarm)
            {
                Prewarm();
            }
        }

        public void Prewarm(Func<T> overrideCreateFunction = null)
        {
            if(overrideCreateFunction != null)
            {
                for(int i = _list.Count; i < _list.Capacity; i++)
                {
                    _list.Add(overrideCreateFunction());
                }
            }
            else
            {
                for(int i = _list.Count; i < _list.Capacity; i++)
                {
                    _list.Add(CreateFunction());
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(Action<T> overrideOnGetAction = null)
        {
            if(CountAvailable == 0)
            {
                _list.Add(CreateFunction());
            }

            var index = _list.Count - 1;
            var element = _list[index];
            _list.RemoveAt(index);

            if(overrideOnGetAction != null)
            {
                overrideOnGetAction.Invoke(element);
            }
            else
            {
                ActionOnGet?.Invoke(element);
            }

            return element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release(T element, Action<T> overrideOnReleaseAction = null)
        {
            #if UNITY_EDITOR || DEVELOPMENT
            for(int i = 0; i < _list.Count; i++)
            {
                if(ReferenceEquals(element, _list[i]))
                {
                    this.LogWarning("Trying to release an object that has already been released to the pool");
                    return;
                }
            }
            #endif

            if(overrideOnReleaseAction != null)
            {
                overrideOnReleaseAction.Invoke(element);
            }
            else
            {
                ActionOnRelease?.Invoke(element);
            }

            _list.Add(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Include(T element)
        {
            _list.Add(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncludeMany(IEnumerable<T> elements)
        {
            _list.AddRange(elements);
        }

        public void Clear()
        {
            if(ActionOnDestroy != null)
            {
                for(int i = 0; i < _list.Count; i++)
                {
                    ActionOnDestroy(_list[i]);
                }
            }

            _list.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
