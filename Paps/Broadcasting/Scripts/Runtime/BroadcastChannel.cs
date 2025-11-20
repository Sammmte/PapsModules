using System;
using System.Collections.Generic;

namespace Paps.Broadcasting
{
    public class BroadcastChannel
    {
        private abstract class DelegateList { }

        private class DelegateList<T> : DelegateList where T : IBroadcastEvent
        {
            private Action<T> _listeners;

            public void Add(Action<T> listener)
            {
                _listeners += listener;
            }

            public void Remove(Action<T> listener)
            {
                _listeners -= listener;
            }

            public void Call(T ev)
            {
                _listeners.Invoke(ev);
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

        public void Unsubscribe<T>(Action<T> listener) where T : IBroadcastEvent
        {
            var type = typeof(T);

            if(_listenersByType.ContainsKey(type))
            {
                var list = (DelegateList<T>)_listenersByType[type];

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