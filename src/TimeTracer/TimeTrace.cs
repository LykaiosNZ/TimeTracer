using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TimeTracer.Timing;

namespace TimeTracer
{
    /// <summary>
    /// Provides functionality for instrumenting sections of code.
    /// </summary>
    public class TimeTrace : IDisposable
    {
        private static readonly AsyncLocal<TimeTrace> _current = new AsyncLocal<TimeTrace>();

        private readonly AsyncLocal<TraceScope> _currentScope = new AsyncLocal<TraceScope>();
        private readonly ConcurrentDictionary<string, ScopeMetrics> _metrics = new ConcurrentDictionary<string, ScopeMetrics>();
        private readonly TimeTrace _previous;
        private readonly ITraceTimer _timer;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeTrace"/> class using a <see cref="StopwatchTraceTimer"/>. <see cref="Current"/> will be set to the new instance.
        /// </summary>
        public TimeTrace()
            : this(new StopwatchTraceTimer())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeTrace"/> class using a specified timer.
        /// </summary>
        /// <param name="timer">Timer to use for measurements.</param>
        public TimeTrace(ITraceTimer timer)
        {
            _previous = _current.Value;
            _current.Value = this;
            _timer = timer;
            _timer.Start();
        }

        /// <summary>
        /// Gets the current <see cref="TimeTrace"/> instance.
        /// </summary>
        public static TimeTrace Current => _current.Value;

        /// <summary>
        /// Gets or sets a value indicating whether timing is enabled.
        /// </summary>
        public static bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets a collection containing the metrics collected by the tracer.
        /// </summary>
        public IReadOnlyCollection<IScopeMetrics> Metrics => _metrics.Values.OrderBy(m => m.Name).ToList();

        /// <summary>
        /// Gets the total duration the tracer has been running for.
        /// </summary>
        public TimeSpan TotalDuration => _timer.Elapsed;

        /// <summary>
        /// Begins a new timing scope for the current tracer.
        /// </summary>
        /// <param name="name">Name of the scope.</param>
        /// <returns>An <see cref="IDisposable"/> respresenting the scope if a current tracer exists and timing is enabled, otherwise null.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
        public static ITraceScope BeginScope(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(name));
            }

            if (!Enabled || _current.Value == null)
            {
                return null;
            }

            return _current.Value.CreateNewScope(name);
        }

        /// <summary>
        /// Disposes the current tracer instance and restores the previous one.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Creates a new scope within the trace.
        /// </summary>
        /// <param name="name">Name of the scope.</param>
        /// <returns>A new trace scope.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
        protected virtual ITraceScope CreateNewScope(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            return new TraceScope(this, name);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer.Stop();
                _current.Value = _previous;
                _currentScope.Value?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Called when a child scope is created.
        /// </summary>
        /// <param name="scope">The created scope.</param>
        protected virtual void OnScopeCreated(ITraceScope scope)
        {
        }

        /// <summary>
        /// Called when a child scope is disposed.
        /// </summary>
        /// <param name="scope">The disposed scope.</param>
        protected virtual void OnScopeDisposed(ITraceScope scope)
        {
        }

        /// <summary>
        /// Updates the metrics for a scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <param name="elapsedNs">Elapsed ticks.</param>
        protected void UpdateMetrics(string scopeName, long elapsedNs)
        {
            ScopeMetrics metric = _metrics.GetOrAdd(scopeName, key => new ScopeMetrics(key));

            metric.Add(elapsedNs);
        }

        protected class TraceScope : ITraceScope
        {
            private readonly string _name;
            private readonly long _startNs;
            private readonly TimeTrace _trace;

            private bool _disposed;
            private long? _endNs;

            /// <summary>
            /// Initialises a new instance of the <see cref="TraceScope"/> class.
            /// </summary>
            /// <param name="trace">Trace that the scope belongs to.</param>
            /// <param name="name">Name of the scope.</param>
            public TraceScope(TimeTrace trace, string name)
            {
                _name = name;
                _startNs = trace._timer.ElapsedNanoseconds;
                _trace = trace;
                Parent = trace._currentScope.Value;
                trace._currentScope.Value = this;
                trace.OnScopeCreated(this);
            }

            /// <inheritdoc />
            public TimeSpan Duration => TimeSpanUtil.FromNanoseconds(ElapsedNanoseconds);

            /// <summary>
            /// Gets the stopwatch ticks since the scope was created.
            /// </summary>
            public long ElapsedNanoseconds => (_endNs ?? _trace._timer.ElapsedNanoseconds) - _startNs;

            /// <inheritdoc />
            public string Name => Parent != null ? $"{Parent.Name}/{_name}" : _name;

            /// <summary>
            /// Gets the parent scope.
            /// </summary>
            public TraceScope Parent { get; }

            public void Dispose() => Dispose(true);

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    _endNs = _trace._timer.ElapsedNanoseconds;
                    _trace.UpdateMetrics(Name, ElapsedNanoseconds);
                    _trace._currentScope.Value = Parent;
                    _trace.OnScopeDisposed(this);
                }

                _disposed = true;
            }
        }
    }
}