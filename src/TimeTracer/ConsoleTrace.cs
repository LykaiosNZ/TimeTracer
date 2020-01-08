using System;
using System.Collections.Generic;
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
        /// <param name="scopeFormatter">Formatter to use for writing scope creation information. If null, scope creation/disposal messages will not be written to the console.</param>
        public ConsoleTrace(IMetricFormatter metricFormatter, IScopeFormatter scopeFormatter)
        {
            _metricFormatter = metricFormatter;
            _scopeFormatter = scopeFormatter;

            WriteMessage("TRACE", "Created");
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (_metricFormatter != null)
            {
                WriteMetrics(Metrics);
            }

            WriteMessage("TRACE", $"Disposed. Total Duration: {TotalDuration}");

            _disposed = true;
        }

        /// <inheritdoc />
        protected virtual string FormatMessage(string title, string message) => $"[TimeTrace] {title} - {message}";

        /// <inheritdoc />
        protected override void OnScopeCreated(ITraceScope scope)
        {
            base.OnScopeCreated(scope);

            if (_scopeFormatter == null)
            {
                return;
            }

            var formattedMessage = _scopeFormatter.FormatCreatedMessage(scope);

            WriteMessage("SCOPE", formattedMessage);
        }

        /// <inheritdoc />
        protected override void OnScopeDisposed(ITraceScope scope)
        {
            base.OnScopeDisposed(scope);

            if (_scopeFormatter == null)
            {
                return;
            }

            var formattedMessage = _scopeFormatter.FormatDisposedMessage(scope);

            WriteMessage("SCOPE", formattedMessage);
        }

        /// <inheritdoc />
        protected void WriteMessage(string title, string message)
        {
            var formattedMessage = FormatMessage(title, message);

            Console.WriteLine(formattedMessage);
        }

        /// <summary>
        /// Writes trace metrics to the console.
        /// </summary>
        /// <param name="metrics">Collection of metrics to write.</param>
        protected void WriteMetrics(IEnumerable<IScopeMetrics> metrics)
        {
            foreach (var metric in Metrics)
            {
                var formattedMessage = _metricFormatter.FormatMessage(metric);

                WriteMessage("METRIC", formattedMessage);
            }
        }
    }
}