using System;

namespace TimeTracer.Formatting
{
    public class DefaultScopeFormatter : IScopeFormatter
    {
        public string FormatCreatedMessage(ITraceScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return $"{scope.Name} Created";
        }

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