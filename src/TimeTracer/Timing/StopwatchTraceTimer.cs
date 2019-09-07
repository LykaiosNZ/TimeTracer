using System;
using System.Diagnostics;

namespace TimeTracer.Timing
{
    /// <summary>
    /// Trace timer that uses a stopwatch to keep track of elapsed time.
    /// </summary>
    public class StopwatchTraceTimer : ITraceTimer
    {
        private static readonly long Factor = 1_000_000_000L / Stopwatch.Frequency;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <inheritdoc/>
        public TimeSpan Elapsed => _stopwatch.Elapsed;

        /// <inheritdoc/>
        public long ElapsedNanoseconds => _stopwatch.ElapsedTicks * Factor;

        /// <inheritdoc/>
        public void Start() => _stopwatch.Start();

        /// <inheritdoc/>
        public void Stop() => _stopwatch.Stop();
    }
}