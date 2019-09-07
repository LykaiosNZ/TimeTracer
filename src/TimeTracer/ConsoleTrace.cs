using System;
using TimeTracer.Formatting;

namespace TimeTracer
{
    /// <summary>
    /// A tracer that writes messages to the console.
    /// </summary>
    public class ConsoleTrace : TimeTrace
    {
        private readonly IMetricFormatter _metricFormatter;
        private readonly IScopeFormatter _scopeFormatter;
        private bool _disposed;

        /// <summary>
        /// Initialise a new instance of the <see cref="ConsoleTrace"/> class using the default formatters.
        /// </summary>
        public ConsoleTrace()
            : this(new DefaultMetricFormatter(), new DefaultScopeFormatter())
        {
        }

        /// <summary>
        /// Initialise a new instance of the <see cref="ConsoleTrace"/> class using specified formatters.
        /// </summary>
        /// <param name="metricFormatter">Formatter to use for writing metrics. If null, metrics will not be written to the console.</param>
        /// <param name="scopeFormatter">Formatter to use for writing scope creation information. If null, scope creation/disposal </param>
        public ConsoleTrace(IMetricFormatter metricFormatter, IScopeFormatter scopeFormatter)
        {
            _metricFormatter = metricFormatter;
            _scopeFormatter = scopeFormatter;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (_metricFormatter != null)
            {
                foreach (var metric in Metrics)
                {
                    Console.WriteLine(_metricFormatter.FormatMessage(metric));
                }
            }

            _disposed = true;
        }

        protected override void OnScopeCreated(ITraceScope scope)
        {
            base.OnScopeCreated(scope);

            if (_scopeFormatter == null)
            {
                return;
            }

            Console.WriteLine(_scopeFormatter.FormatCreatedMessage(scope));
        }

        protected override void OnScopeDisposed(ITraceScope scope)
        {
            base.OnScopeDisposed(scope);

            if (_scopeFormatter == null)
            {
                return;
            }

            Console.WriteLine(_scopeFormatter.FormatDisposedMessage(scope));
        }
    }
}