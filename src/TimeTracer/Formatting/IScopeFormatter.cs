namespace TimeTracer
{
    public interface IScopeFormatter
    {
        string FormatCreatedMessage(ITraceScope scope);

        string FormatDisposedMessage(ITraceScope scope);
    }
}