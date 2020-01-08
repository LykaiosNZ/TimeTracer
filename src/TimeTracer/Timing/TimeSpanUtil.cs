using System;

namespace TimeTracer.Timing
{
    /// <summary>
    /// Utility class for interacting with <see cref="TimeSpan"/>
    /// </summary>
    public static class TimeSpanUtil
    {
        /// <summary>
        /// Returns a System.TimeSpan that represents a specified number of nanoseconds.
        /// </summary>
        /// <param name="value">Number of nanoseconds</param>
        /// <returns>An object that represents value.</returns>
        public static TimeSpan FromNanoseconds(long value)
        {
            if (value == 0)
            {
                return TimeSpan.Zero;
            }

            double milliseconds = value / 1_000_000D;

            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}