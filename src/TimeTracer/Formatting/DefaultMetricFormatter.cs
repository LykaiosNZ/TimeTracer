using System;

namespace TimeTracer.Formatting
{
    public class DefaultMetricFormatter : IMetricFormatter
    {
        public string FormatMessage(IScopeMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            return $"Scope {metrics.Name}: Total Time Elapsed {metrics.TotalDuration}, Creation Count {metrics.Count}";
        }
    }
}