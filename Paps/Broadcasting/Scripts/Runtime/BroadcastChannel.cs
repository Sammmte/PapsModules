using System;
using System.Collections.Generic;

namespace Paps.Broadcasting
{
    public class BroadcastChannel
    {
        private class DelegateList
        {
            private List<Action> _voidListeners = new List<Action>();

            public void Add(Action listener)
            {
                _voidListeners.Add(listener);
            }

            public void Remove(Action listener)
            {
                _voidListeners.Remove(listener);
            }

            public void Call()
            {
                foreach (var listener in _voidListeners)
                    listener();
            }
        }

        private class DelegateList<T> : DelegateList where T : IBroadcastEvent
        {
            private List<Action<T>> _listeners = new List<Action<T>>();

            public void Add(Action<T> listener)
            {
                _listeners.Add(listener);
            }

            public void Remove(Action<T> listener)
            {
                _listeners.Remove(listener);
            }

            public void Call(T ev)
            {
                Call();

                foreach(var listener in _listeners)
                    listener(ev);
            }
        }

        public static BroadcastChannel Global { get; } = new BroadcastChannel();

        private Dictionary<Type, DelegateList> _listenersByType = new Dictionary<Type, DelegateList>();

        public void Subscribe<T>(Action<T> listener) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if (!_listenersByType.ContainsKey(type))
                _listenersByType.Add(type, new DelegateList<T>());

            var list = (DelegateList<T>)_listenersByType[type];

            list.Add(listener);
        }

        public void Subscribe<T>(Action listener) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if (!_listenersByType.ContainsKey(type))
                _listenersByType.Add(type, new DelegateList<T>());

            var list = (DelegateList<T>)_listenersByType[type];

            list.Add(listener);
        }

        public void Subscribe(Type type, Action listener)
        {
            if (!_listenersByType.ContainsKey(type))
            {
                var newType = typeof(DelegateList<>).MakeGenericType(type);
                var newDelegateList = (DelegateList)Activator.CreateInstance(newType);
                _listenersByType.Add(type, newDelegateList);
            }

            var list = _listenersByType[type];

            list.Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if(_listenersByType.ContainsKey(type))
            {
                var list = (DelegateList<T>)_listenersByType[type];

                list.Remove(listener);
            }
        }

        public void Unsubscribe<T>(Action listener) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if(_listenersByType.ContainsKey(type))
            {
                var list = (DelegateList<T>)_listenersByType[type];

                list.Remove(listener);
            }
        }

        public void Unsubscribe(Type type, Action listener)
        {
            if (_listenersByType.ContainsKey(type))
            {
                var list = _listenersByType[type];

                list.Remove(listener);
            }
        }

        public void Raise<T>(T @event) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if(_listenersByType.ContainsKey(type))
            {
                var list = (DelegateList<T>)_listenersByType[type];

                list.Call(@event);
            }
        }
    }
}