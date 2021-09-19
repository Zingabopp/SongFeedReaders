using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// Basic implemenation of <see cref="ILoggerSettings"/>.
    /// </summary>
    public class LoggerSettings : ILoggerSettings
    {
        /// <inheritdoc/>
        public string? ModuleName { get; set; }
        /// <inheritdoc/>
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        /// <inheritdoc/>
        public bool ShowModule { get; set; }
        /// <inheritdoc/>
        public bool ShortSource { get; set; }
        /// <inheritdoc/>
        public bool EnableTimeStamp { get; set; } = true;
        /// <summary>
        /// Creates a new <see cref="LoggerSettings"/>.
        /// </summary>
        public LoggerSettings() { }
        /// <summary>
        /// Creates a new <see cref="LoggerSettings"/>, 
        /// cloning the values from another <see cref="ILoggerSettings"/>.
        /// </summary>
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
