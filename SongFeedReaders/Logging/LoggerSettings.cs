using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Logging
{
    public class LoggerSettings : ILoggerSettings
    {
        public string? ModuleName { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        public bool ShowModule { get; set; }
        public bool ShortSource { get; set; }
        public bool EnableTimeStamp { get; set; } = true;

        public LoggerSettings() { }
        public LoggerSettings(ILoggerSettings loggerSettings)
        {
            ModuleName = loggerSettings.ModuleName;
            LogLevel = loggerSettings.LogLevel;
            ShowModule = loggerSettings.ShowModule;
            ShortSource = loggerSettings.ShortSource;
            EnableTimeStamp = loggerSettings.EnableTimeStamp;
        }
    }
}
