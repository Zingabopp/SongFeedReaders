using SongFeedReaders.Utilities;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SongFeedReaders.Logging
{
    public class FeedReaderLogger : ILogger
    {
        protected LoggerSettings loggerSettings;

        protected FeedReaderLogger()
        {
            loggerSettings = new LoggerSettings();
        }

        protected FeedReaderLogger(ILoggerSettings settings)
        {
            loggerSettings = settings as LoggerSettings ?? new LoggerSettings(settings);
        }

        public string? ModuleName
        {
            get { return loggerSettings.ModuleName; }
            set { loggerSettings.ModuleName = value; }
        }
        public LogLevel LogLevel
        {
            get { return loggerSettings.LogLevel; }
            set { loggerSettings.LogLevel = value; }
        }
        public bool ShowModule
        {
            get { return loggerSettings.ShowModule; }
            set { loggerSettings.ShowModule = value; }
        }
        public bool ShortSource
        {
            get { return loggerSettings.ShortSource; }
            set { loggerSettings.ShortSource = value; }
        }
        public bool EnableTimestamp
        {
            get => loggerSettings.EnableTimeStamp;
            set => loggerSettings.EnableTimeStamp = value;
        }


        public void Log(string message, LogLevel logLevel, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
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

        public void Log(Exception e, LogLevel logLevel, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
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

