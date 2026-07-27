using System;
using System.Collections.Generic;

namespace Paps.UnityExtensions
{
    public static class EnumUtils
    {
        private static Dictionary<Type, int> _enumValuesCount = new Dictionary<Type, int>();
        private static Dictionary<Type, Array> _enumValues = new Dictionary<Type, Array>();
        
        public static int GetCountOf<T>() where T : struct, Enum
        {
            var type = typeof(T);

            if (_enumValuesCount.TryGetValue(type, out var value))
            {
                return value;
            }

            _enumValuesCount[type] = Enum.GetValues(type).Length;

            return _enumValuesCount[type];
        }

        public static T[] GetValues<T>() where T : struct, Enum
        {
             var type = typeof(T);

            if (_enumValues.TryGetValue(type, out var value))
            {
                return (T[])value;
            }

            _enumValues[type] = Enum.GetValues(type);

            return (T[])_enumValues[type];
        }
    }
}