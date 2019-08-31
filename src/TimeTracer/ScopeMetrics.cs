using System;
using System.Threading;

namespace TimeTracer
{
    public class ScopeMetrics : IScopeMetrics
    {
        private int _count;
        private long _elapsedTicks;

        public ScopeMetrics(string name)
        {
            Name = name;
        }

        public int Count => _count;
        public string Name { get; }
        public TimeSpan TotalDuration => TimeSpan.FromTicks(_elapsedTicks);

        public void AddTicks(long incrementBy)
        {
            Interlocked.Add(ref _elapsedTicks, incrementBy);
        }

        public void IncrementCount()
        {
            Interlocked.Increment(ref _count);
        }
    }
}