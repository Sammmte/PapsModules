using System;
using System.Collections.Generic;

namespace Paps.UnityExtensions
{
    public static class EnumUtils
    {
        private static Dictionary<Type, int> _enumData = new Dictionary<Type, int>();
        
        public static int GetCountOf<T>() where T : struct, Enum
        {
            var type = typeof(T);

            if (_enumData.TryGetValue(type, out int value))
            {
                return value;
            }

            _enumData[type] = Enum.GetValues(type).Length;

            return _enumData[type];
        }
    }
}