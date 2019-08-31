using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TimeTracer
{
    public class TimeTrace : IDisposable
    {
        private static readonly AsyncLocal<TimeTrace> _current = new AsyncLocal<TimeTrace>();

        private readonly AsyncLocal<TraceScope> _currentScope = new AsyncLocal<TraceScope>();
        private readonly ConcurrentDictionary<string, ScopeMetrics> _metrics = new ConcurrentDictionary<string, ScopeMetrics>();
        private readonly TimeTrace _previous;
        private readonly Stopwatch _stopwatch;

        private bool _disposed;

        public TimeTrace()
        {
            _previous = _current.Value;
            _current.Value = this;
            _stopwatch = Stopwatch.StartNew();
        }

        public static TimeTrace Current => _current.Value;
        public static bool Enabled { get; set; } = true;
        public IReadOnlyCollection<IScopeMetrics> Metrics => _metrics.Values.OrderBy(m => m.Name).ToList();
        public TimeSpan TotalDuration => TimeSpan.FromTicks(_stopwatch.ElapsedTicks);

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

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _stopwatch.Stop();
            _current.Value = _previous;
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

            metric.IncrementCount();
            metric.AddTicks(elapsedTicks);
        }
    }
}