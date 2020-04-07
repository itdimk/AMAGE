using System;

namespace AMAGE.Common.Extensions
{
    public static class MathEx
    {
        public static bool Clamped<T>(this T value, T min, T max)
            where T : IComparable
        {
            if (max.CompareTo(min) > 0)
                return value.CompareTo(max) <= 0 && value.CompareTo(min) >= 0;
            else
                return value.CompareTo(max) >= 0 && value.CompareTo(min) <= 0;
        }

        public static bool Clamped(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool Clamped(this long value, long min, long max)
        {
            return value >= min && value <= max;
        }

        public static bool Clamped(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static bool Clamped(this double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        public static T Clamp<T>(this T value, T min, T max)
            where T : IComparable
        {
            if (max.CompareTo(min) < 0)
            {
                if (value.CompareTo(max) > 0)
                    return max;

                if (value.CompareTo(min) < 0)
                    return min;
            }
            else
            {
                if (value.CompareTo(max) > 0)
                    return max;

                if (value.CompareTo(min) < 0)
                    return min;
            }

            return value;
        }

        public static int Clamp(this int value, int min, int max)
        {
            return value < min ? min
                : value > max ? max
                : value;
        }

        public static long Clamp(this long value, long min, long max)
        {
            return value < min ? min
                : value > max ? max
                : value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return value < min ? min
                : value > max ? max
                : value;
        }

        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min
                : value > max ? max
                : value;
        }
    }
}
