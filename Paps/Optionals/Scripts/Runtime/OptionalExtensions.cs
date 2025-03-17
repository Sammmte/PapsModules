using System;

namespace Paps.Optionals
{
    public static class OptionalExtensions
    {
        public static Optional<T> AsOptional<T>(this T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> WhenPresent<T>(this Optional<T> optional, Action<T> action)
        {
            if(optional.HasValue)
                action(optional.Value);

            return optional;
        }

        public static Optional<T> WhenAbsent<T>(this Optional<T> optional, Action action)
        {
            if (!optional.HasValue)
                action();

            return optional;
        }

        public static T ValueOrDefault<T>(this Optional<T> optional, T defaultValue = default)
        {
            return optional.HasValue ? optional.Value : defaultValue;
        }
    }
}