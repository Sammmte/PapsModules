using System;
using System.Linq;

namespace Paps.UnityExtensions
{
    public static class EnumFlagsExtensions
    {
        public static T AddFlags<T>(this T enumValue, params T[] flags) where T : struct, Enum
        {
            int intValue = Convert.ToInt32(enumValue);

            int[] flagsAsInts = flags.Select(flag => Convert.ToInt32(flag)).ToArray();

            foreach(var flag in flagsAsInts)
            {
                intValue |= flag;
            }

            return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), intValue));
        }

        public static T RemoveFlags<T>(this T enumValue, params T[] flags) where T : struct, Enum
        {
            int intValue = Convert.ToInt32(enumValue);

            int[] flagsAsInts = flags.Select(flag => Convert.ToInt32(flag)).ToArray();

            foreach (var flag in flagsAsInts)
            {
                intValue &= (~flag);
            }

            return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), intValue));
        }
    }
}