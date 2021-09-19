using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReaders.Logging
{
    public abstract class FeedReaderLoggerBase : ILogger
    {
        private ILoggerSettings l;

        protected FeedReaderLoggerBase()
        {
            l = new LoggerSettings();
        }

        protected FeedReaderLoggerBase(ILoggerSettings settings)
        {
            l = settings ?? new LoggerSettings();
        }

        public string? ModuleName
        {
            get { return l.ModuleName; }
            set { l.ModuleName = value; }
        }
        public LogLevel LogLevel
        {
            get { return l.LogLevel; }
            set { l.LogLevel = value; }
        }
        public bool ShortSource
        {
            get { return l.ShortSource; }
            set { l.ShortSource = value; }
        }
        public bool EnableTimestamp
        {
            get => l.EnableTimeStamp;
            set => l.EnableTimeStamp = value;
        }

        public abstract void Log(string message, LogLevel level, string file, string member, int line);
        public abstract void Log(Exception e, LogLevel level, string file, string member, int line);
    }
}

