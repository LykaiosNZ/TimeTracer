using System;
using TimeTracer.Timing;

namespace TimeTracer
{
    public class ScopeMetrics : IScopeMetrics
    {
        private readonly object _lockObj = new object();

        /// <summary>
        /// Initialises a new instance of the <see cref="ScopeMetrics"/> class.
        /// </summary>
        /// <param name="name">Name of the scope the metrics are for.</param>
        public ScopeMetrics(string name)
        {
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
        /// Adds information about a scope instance to the metrics.
        /// </summary>
        /// <param name="elapsedNs">Duration of the scope in nanoseconds.</param>
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