using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeTracer
{
    public class TimeTrace : IDisposable
    {
        public static bool Enabled { get; set; }
        private static readonly AsyncLocal<TimeTrace> _current = new AsyncLocal<TimeTrace>();

        private readonly TimeTrace _previous;
        private readonly Stopwatch _stopwatch;
        private readonly AsyncLocal<TraceScope> _currentScope = new AsyncLocal<TraceScope>();
        private readonly ConcurrentDictionary<string, ScopeMetrics> _metrics = new ConcurrentDictionary<string, ScopeMetrics>();
        private bool _disposed;

        public TimeTrace()
        {
            _previous = _current.Value;
            _current.Value = this;
            _stopwatch = Stopwatch.StartNew();
        }

        public static TimeTrace Current => _current.Value;
        public IReadOnlyCollection<IScopeMetrics> Metrics => _metrics.Values.ToList();
        public TimeSpan TotalDuration => TimeSpan.FromTicks(_stopwatch.ElapsedTicks);

        public static IDisposable BeginScope(string name)
        {
            if (string.IsNullOrEmpty(name))
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
            var currentScope = _currentScope.Value;

            var newScope = new TraceScope(currentScope, _stopwatch.ElapsedTicks, name, OnScopeDisposed);

            AddPlaceholderMetrics(newScope);

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

        private void AddPlaceholderMetrics(TraceScope scope) => _metrics.GetOrAdd(scope.PrefixedName, key => new ScopeMetrics(key));

        private void UpdateMetrics(string prefixedName, long elapsedTicks)
        {
            var metric = _metrics[prefixedName];

            metric.IncrementCount();
            metric.AddTicks(elapsedTicks);
        }
    }
}
