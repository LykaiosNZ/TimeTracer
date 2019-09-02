using System;
using System.Threading;

namespace TimeTracer
{
    public class ScopeMetrics : IScopeMetrics
    {
        private readonly object _lockObj;
        private long _totalTicks;

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
        public TimeSpan TotalDuration => TimeSpan.FromTicks(_totalTicks);

        /// <summary>
        /// Adds information about a scope instance to the metrics.
        /// </summary>
        /// <param name="elapsedTicks">Duration of the scope.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="elapsedTicks"/> is less than 0.</exception>
        public void Add(long elapsedTicks)
        {
            if (elapsedTicks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTicks), elapsedTicks, "Cannot be less than 0");
            }

            lock(_lockObj)
            {
                _totalTicks += elapsedTicks;
                Count++;
            }
        }
    }
}