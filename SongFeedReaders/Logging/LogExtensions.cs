using System;
using System.Runtime.CompilerServices;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// Extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs a message using <see cref="LogLevel.Trace"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Trace(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Trace, file, member, line);
        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Trace"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Trace(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Trace, file, member, line);
        /// <summary>
        /// Logs a message using <see cref="LogLevel.Debug"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Debug(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Debug, file, member, line);
        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Debug"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Debug(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Debug, file, member, line);
        /// <summary>
        /// Logs a message using <see cref="LogLevel.Info"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Info(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Info, file, member, line);
        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Info"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Info(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Info, file, member, line);
        /// <summary>
        /// Logs a message using <see cref="LogLevel.Warning"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Warning(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Warning, file, member, line);
        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Warning"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Warning(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Warning, file, member, line);
        /// <summary>
        /// Logs a message using <see cref="LogLevel.Error"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Error(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Error, file, member, line);
        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Error"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Error(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Error, file, member, line);

        /// <summary>
        /// Logs an exception using <see cref="LogLevel.Exception"/>.
        /// Parameters <paramref name="file"/>, <paramref name="member"/>, <paramref name="line"/> 
        /// are auto-generated if no value is given for them.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <param name="member"></param>
        /// <param name="line"></param>
        public static void Exception(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Exception, file, member, line);

    }
}
