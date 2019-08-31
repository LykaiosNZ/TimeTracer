using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
        private readonly Stopwatch _stopwatch;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeTrace"/> class. <see cref="Current"/> will be set to the new instance.
        /// </summary>
        public TimeTrace()
        {
            _previous = _current.Value;
            _current.Value = this;
            _stopwatch = Stopwatch.StartNew();
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
        public TimeSpan TotalDuration => TimeSpan.FromTicks(_stopwatch.ElapsedTicks);

        /// <summary>
        /// Begins a new timing scope for the current tracer.
        /// </summary>
        /// <param name="name">Name of the scope.</param>
        /// <returns>An <see cref="IDisposable"/> respresenting the scope if a current tracer exists and timing is enabled, otherwise null.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
        public static IDisposable BeginScope(string name)
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
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _stopwatch.Stop();
            _current.Value = _previous;

            _disposed = true;
        }

        private TraceScope CreateNewScope(string name)
        {
            TraceScope currentScope = _currentScope.Value;

            var newScope = new TraceScope(currentScope, _stopwatch.ElapsedTicks, name, OnScopeDisposed);

            _currentScope.Value = newScope;

            return newScope;
        }

        private void OnScopeDisposed(TraceScope scope)
        {
            long endTicks = _stopwatch.ElapsedTicks;
            long elapsedTicks = endTicks - scope.StartTicks;

            UpdateMetrics(scope.PrefixedName, elapsedTicks);

            _currentScope.Value = scope.Parent;
        }

        private void UpdateMetrics(string prefixedName, long elapsedTicks)
        {
            ScopeMetrics metric = _metrics.GetOrAdd(prefixedName, key => new ScopeMetrics(key));

            metric.Add(elapsedTicks);
        }
    }
}