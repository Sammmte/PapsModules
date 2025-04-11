using System;
using System.Collections.Generic;

namespace Paps.GlobalProvisioning
{
    public static class Locator
    {
        private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private static readonly Dictionary<string, object> _namedInstances = new Dictionary<string, object>();

        public static void Create<T>(T instance) where T : class
        {
            Create(typeof(T), instance);
        }

        public static void Create(Type type, object instance)
        {
            _instances[type] = instance;
        }

        public static T Locate<T>() where T : class
        {
            if (_instances.ContainsKey(typeof(T)))
                return (T)_instances[typeof(T)];

            return null;
        }

        public static void CreateWithName<T>(T instance, string name) where T : class
        {
            _namedInstances[name] = instance;
        }

        public static T Locate<T>(string name) where T : class
        {
            if(_namedInstances.ContainsKey(name))
                return (T)_namedInstances[name];

            return null;
        }

        public static void Remove<T>() where T : class
        {
            Remove(typeof(T));
        }

        public static void Remove(Type type)
        {
            _instances.Remove(type);
        }

        public static void Remove(string name)
        {
            _namedInstances.Remove(name);
        }
    }
}