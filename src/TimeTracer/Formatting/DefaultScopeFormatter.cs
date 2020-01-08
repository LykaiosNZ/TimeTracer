using System;

namespace TimeTracer.Formatting
{
    /// <summary>
    /// Default trace scope formatter.
    /// </summary>
    public class DefaultScopeFormatter : IScopeFormatter
    {
        /// <inheritdoc />
        public string FormatCreatedMessage(ITraceScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return $"{scope.Name} Created";
        }

        /// <inheritdoc />
        public string FormatDisposedMessage(ITraceScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return $"{scope.Name} Disposed";
        }
    }
}