namespace TimeTracer
{
    /// <summary>
    /// Formats trace scope messages.
    /// </summary>
    public interface IScopeFormatter
    {
        /// <summary>
        /// Formats a message for scope creation.
        /// </summary>
        /// <param name="scope">Created scope.</param>
        /// <returns>Formatted message</returns>
        string FormatCreatedMessage(ITraceScope scope);

        /// <summary>
        /// Formats a message for scope disposal.
        /// </summary>
        /// <param name="scope">Disposed scope.</param>
        /// <returns>Formatted message.</returns>
        string FormatDisposedMessage(ITraceScope scope);
    }
}