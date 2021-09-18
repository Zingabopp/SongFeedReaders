using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReaders.Logging
{
    public abstract class FeedReaderLoggerBase : ILogger
    {
        private string? _loggerName;
        private LogLevel? _logLevel;
        private bool? _shortSource;
        private bool? _enableTimeStamp;

        public string? LoggerName
        {
            get { return _loggerName ?? LogController?.LoggerName; }
            set { _loggerName = value; }
        }
        public LogLevel LogLevel
        {
            get { return _logLevel ?? LogController?.LogLevel ?? LogLevel.Disabled; }
            set { _logLevel = value; }
        }
        public bool ShortSource
        {
            get { return _shortSource ?? LogController?.ShortSource ?? false; }
            set { _shortSource = value; }
        }
        public bool EnableTimestamp
        {
            get { return _enableTimeStamp ?? LogController?.EnableTimestamp ?? true; }
            set { _enableTimeStamp = value; }
        }
        private LoggingController? _loggingController;
        public LoggingController LogController
        {
            get { return _loggingController ?? LoggingController.DefaultLogController; }
            set
            {
                if (_loggingController == value)
                    return;
                _loggingController = value;
            }
        }

        public abstract void Log(string message, LogLevel level, string file, string member, int line);
        public abstract void Log(Exception e, LogLevel level, string file, string member, int line);
    }
}

