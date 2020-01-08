using System;

namespace TimeTracer.Timing
{
    /// <summary>
    /// Provides timing for traces.
    /// </summary>
    public interface ITraceTimer
    {
        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, measured in nanoseconds.
        /// </summary>
        long ElapsedNanoseconds { get; }

        /// <summary>
        /// Starts or resumes measuring elapsed time.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops measuring elapsed time.
        /// </summary>
        void Stop();
    }
}