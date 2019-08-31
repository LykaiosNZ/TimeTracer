using System;
using System.Diagnostics;

namespace TimeTracer
{
    public class TraceScope : IDisposable
    {
        private readonly Action<TraceScope> _onDispose;
        private bool _disposed;

        public TraceScope(TraceScope parent, long startTicks, string name, Action<TraceScope> onDispose)
        {
            if (startTicks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startTicks), startTicks, "Cannot be less than zero");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(name));
            }

            Parent = parent;
            StartTicks = startTicks;
            Name = name;
            _onDispose = onDispose;

            Debug.WriteLine($"TraceScope `{PrefixedName}` Started");
        }

        public string Name { get; }
        public TraceScope Parent { get; }
        public string PrefixedName => Parent != null ? $"{Parent.PrefixedName}/{Name}" : Name;
        public long StartTicks { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _onDispose?.Invoke(this);

            Debug.WriteLine($"TraceScope `{PrefixedName}` Ended");

            _disposed = true;
        }
    }
}