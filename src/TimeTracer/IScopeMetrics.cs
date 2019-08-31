using System;

namespace TimeTracer
{
    public interface IScopeMetrics
    {
        int Count { get; }
        string Name { get; }
        TimeSpan TotalDuration { get; }
    }
}