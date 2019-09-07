namespace TimeTracer
{
    public interface IMetricFormatter
    {
        string FormatMessage(IScopeMetrics metrics);
    }
}