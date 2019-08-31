using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracer
{
    public interface IScopeMetrics
    {
        string Name { get; }
        TimeSpan Elapsed { get; }
        int Count { get; }
    }
}
