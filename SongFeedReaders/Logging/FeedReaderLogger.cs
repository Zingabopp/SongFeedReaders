using SongFeedReaders.Utilities;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// Basic console logger for <see cref="SongFeedReaders"/>.
    /// </summary>
    public class FeedReaderLogger : ILogger
    {
        /// <summary>
        /// Settings for the logger.
        /// </summary>
        protected readonly LoggerSettings loggerSettings;

        /// <summary>
        /// Creates a new <see cref="FeedReaderLogger"/>.
        /// </summary>
        public FeedReaderLogger()
        {
            loggerSettings = new LoggerSettings();
        }
        /// <summary>
        /// Creates a new <see cref="FeedReaderLogger"/>.
        /// </summary>
        /// <param name="settings"></param>
        public FeedReaderLogger(ILoggerSettings settings)
        {
            loggerSettings = settings as LoggerSettings ?? new LoggerSettings(settings);
        }

        /// <summary>
        /// Name of the module using the logger.
        /// </summary>
        public string? ModuleName
        {
            get { return loggerSettings.ModuleName; }
            set { loggerSettings.ModuleName = value; }
        }
        /// <summary>
        /// Lowest <see cref="Logging.LogLevel"/> to output messages from.
        /// </summary>
        public LogLevel LogLevel
        {
            get { return loggerSettings.LogLevel; }
            set { loggerSettings.LogLevel = value; }
        }
        /// <summary>
        /// If true, show the module name in log messages.
        /// </summary>
        public bool ShowModule
        {
            get { return loggerSettings.ShowModule; }
            set { loggerSettings.ShowModule = value; }
        }
        /// <summary>
        /// If true, don't show file/member/line information in the log message.
        /// </summary>
        public bool ShortSource
        {
            get { return loggerSettings.ShortSource; }
            set { loggerSettings.ShortSource = value; }
        }
        /// <summary>
        /// If true, show the current time in the message.
        /// </summary>
        public bool EnableTimestamp
        {
            get => loggerSettings.EnableTimeStamp;
            set => loggerSettings.EnableTimeStamp = value;
        }
        /// <inheritdoc/>
        public void Log(string message, LogLevel logLevel,
            [CallerFilePath] string? file = "",
            [CallerMemberName] string? member = "",
            [CallerLineNumber] int line = 0)
        {
            if (LogLevel > logLevel)
            {
                return;
            }
            string sourcePart, timePart = "";
            if (!ShortSource)
                sourcePart = $"[{Path.GetFileName(file)}_{member}({line})";
            else
                sourcePart = $"[{ModuleName}";
            if (EnableTimestamp)
                timePart = $" @ {Util.Now:HH:mm}";
            Console.WriteLine($"{sourcePart}{timePart} - {logLevel}] {message}");
        }
        /// <inheritdoc/>
        public void Log(Exception e, LogLevel logLevel,
            [CallerFilePath] string? file = "",
            [CallerMemberName] string? member = "",
            [CallerLineNumber] int line = 0)
        {
            if (LogLevel > logLevel)
            {
                return;
            }
            string sourcePart, timePart = "";
            if (!ShortSource)
                sourcePart = $"[{Path.GetFileName(file)}_{member}({line})";
            else
                sourcePart = $"[{ModuleName}";
            if (EnableTimestamp)
                timePart = $" @ {Util.Now:HH:mm}";
            Console.WriteLine($"{sourcePart}{timePart} - {logLevel}] {e.Message}");
            Console.WriteLine(e);
        }
    }
}

