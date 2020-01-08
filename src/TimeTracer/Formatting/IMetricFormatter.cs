namespace TimeTracer
{
    /// <summary>
    /// Formats trace metric messages.
    /// </summary>
    public interface IMetricFormatter
    {
        /// <summary>
        /// Formats an output message for trace metrics.
        /// </summary>
        /// <param name="metrics">Metrics to format a message for.</param>
        /// <returns>Formatted message.</returns>
        string FormatMessage(IScopeMetrics metrics);
    }
}