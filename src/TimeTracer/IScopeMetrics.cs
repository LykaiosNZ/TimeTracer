using System;

namespace TimeTracer
{
    /// <summary>
    /// Metrics for a scope.
    /// </summary>
    public interface IScopeMetrics
    {
        /// <summary>
        /// Gets a count of how many times the scope was created during the trace.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the name of the scope the metrics are for.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the total duration of all instances of the the scope for the trace.
        /// </summary>
        TimeSpan TotalDuration { get; }
    }
}