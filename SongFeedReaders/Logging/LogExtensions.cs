using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReaders.Logging
{
    public static class LogExtensions
    {
        public static void Trace(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Trace, file, member, line);
        public static void Trace(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Trace, file, member, line);
        public static void Debug(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Debug, file, member, line);
        public static void Debug(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Debug, file, member, line);
        public static void Info(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Info, file, member, line);
        public static void Info(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Info, file, member, line);
        public static void Warning(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Warning, file, member, line);
        public static void Warning(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Warning, file, member, line);
        public static void Error(this ILogger logger, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(message, LogLevel.Error, file, member, line);
        public static void Error(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Error, file, member, line);

        public static void Exception(this ILogger logger, Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) => logger.Log(e, LogLevel.Exception, file, member, line);

    }
}
