using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Logging
{
    public class LoggerSettings : ILoggerSettings
    {
        public string? ModuleName { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        public bool ShortSource { get; set; }
        public bool EnableTimeStamp { get; set; } = true;
    }
}
