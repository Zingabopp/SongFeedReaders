namespace SongFeedReaders.Logging
{
    /// <summary>
    /// Settings for an <see cref="ILogger"/>.
    /// </summary>
    public interface ILoggerSettings
    {
        /// <summary>
        /// Name of the module using the logger.
        /// </summary>
        string? ModuleName { get; set; }
        /// <summary>
        /// Lowest <see cref="Logging.LogLevel"/> to output messages from.
        /// </summary>
        LogLevel LogLevel { get; set; }
        /// <summary>
        /// If true, show the module name in log messages.
        /// </summary>
        bool ShowModule { get; set; }
        /// <summary>
        /// If true, don't show file/member/line information in the log message.
        /// </summary>
        bool ShortSource { get; set; }
        /// <summary>
        /// If true, show the current time in the message.
        /// </summary>
        bool EnableTimeStamp { get; set; }
    }
}
