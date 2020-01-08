using System;
using System.Diagnostics;
using TimeTracer.Timing;

namespace TimeTracer
{
    /// <summary>
    /// Metrics for a trace scope.
    /// </summary>
    [DebuggerDisplay("Scope Name: {Name}, Total Duration: {TotalDuration}, Call Count: {Count}")]
    public class ScopeMetrics : IScopeMetrics
    {
        private readonly object _lockObj = new object();

        /// <summary>
        /// Initialises a new instance of the <see cref="ScopeMetrics"/> class.
        /// </summary>
        /// <param name="name">Name of the scope the metrics are for.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
        public ScopeMetrics(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(name));
            }

            Name = name;
        }

        /// <inheritdoc />
        public int Count { get; private set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public TimeSpan TotalDuration => TimeSpanUtil.FromNanoseconds(TotalNanoseconds);

        /// <summary>
        /// Gets the total duration of all instances of the the scope for the trace, in nanoseconds.
        /// </summary>
        public long TotalNanoseconds { get; private set; }

        /// <summary>
        /// Adds the specified time (in nanoseconds) to the total elapsed time for the scope. Also increments the call count for the scope by 1.
        /// </summary>
        /// <param name="elapsedNs">Duration of the scope, in nanoseconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="elapsedNs"/> is less than 0.</exception>
        public void Add(long elapsedNs)
        {
            if (elapsedNs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedNs), elapsedNs, "Cannot be less than 0");
            }

            lock (_lockObj)
            {
                TotalNanoseconds += elapsedNs;
                Count++;
            }
        }
    }
}