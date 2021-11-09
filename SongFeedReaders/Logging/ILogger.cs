using System;
using System.Runtime.CompilerServices;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// Interface for a logger used by <see cref="SongFeedReaders"/>.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message at the given level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        void Log(string message, LogLevel level, [CallerFilePath] string? file = null,
            [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);
        /// <summary>
        /// Logs an <see cref="Exception"/> at the given level.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="level"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        void Log(Exception e, LogLevel level, [CallerFilePath] string? file = null,
            [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);
    }
    /// <summary>
    /// Logging level of a message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace-level messages.
        /// </summary>
        Trace = 0,
        /// <summary>
        /// Debug-level messages.
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Info-level messages.
        /// </summary>
        Info = 2,
        /// <summary>
        /// Warning-level messages.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Error-level messages.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Exception-level messages.
        /// </summary>
        Exception = 5,
        /// <summary>
        /// Logger will not output any messages it's given.
        /// </summary>
        Disabled = 6
    }
}
