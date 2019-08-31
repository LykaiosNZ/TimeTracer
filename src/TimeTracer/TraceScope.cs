using System;
using System.Diagnostics;

namespace TimeTracer
{
    internal class TraceScope : IDisposable
    {
        private readonly Action<TraceScope> _onDispose;
        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="TraceScope"/> class.
        /// </summary>
        /// <param name="parent">This scope's parent scope.</param>
        /// <param name="startTicks">The current stopwatch ticks of the tracer that the scope belongs to.</param>
        /// <param name="name">The name of the scope.</param>
        /// <param name="onDispose">A delegate to be executed when the scope is being disposed.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startTicks"/> is less than 0.</exception>
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

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parent scope.
        /// </summary>
        public TraceScope Parent { get; }

        /// <summary>
        /// Gets the name of the scope including the parent scope's prefixed name.
        /// </summary>
        public string PrefixedName => Parent != null ? $"{Parent.PrefixedName}/{Name}" : Name;

        /// <summary>
        /// Gets the number of ticks on the tracer's stopwatch at the time the scope was created.
        /// </summary>
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