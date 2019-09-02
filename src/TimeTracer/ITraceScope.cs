using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracer
{
    /// <summary>
    /// A scope within a trace.
    /// </summary>
    public interface ITraceScope : IDisposable
    {
        /// <summary>
        /// Name of the scope.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Duration the scope has been active.
        /// </summary>
        TimeSpan Duration { get; }
    }
}
