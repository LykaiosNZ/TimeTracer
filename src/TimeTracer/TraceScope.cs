using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TimeTracer
{
    public class TraceScope : IDisposable
    {
        private bool _disposed;
        private Action<TraceScope> _onDispose;

        public TraceScope(TraceScope parent, long startTicks, string name, Action<TraceScope> onDispose)
        {
            Parent = parent;
            StartTicks = startTicks;
            Name = name;
            _onDispose = onDispose;

            Debug.WriteLine($"TraceScope `{PrefixedName}` Started");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _onDispose(this);

            Debug.WriteLine($"TraceScope `{PrefixedName}` Ended");

            _disposed = true;
        }

        public string PrefixedName => Parent != null ? $"{Parent.PrefixedName}/{Name}" : Name;
        public string Name { get; }
        public long StartTicks { get; }
        public TraceScope Parent { get; }
    }
}
