using System;

namespace TimeTracer.Formatting
{
    /// <summary>
    /// Default trace metric formatter.
    /// </summary>
    public class DefaultMetricFormatter : IMetricFormatter
    {
        /// <inheritdoc />
        public string FormatMessage(IScopeMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            return $"Scope: {metrics.Name}, Total Duration: {metrics.TotalDuration}, Count: {metrics.Count}";
        }
    }
}