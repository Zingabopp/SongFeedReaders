using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Logging
{
    public interface ILogger
    {
        void Log(string message, LogLevel level, string file, string member, int line);
        void Log(Exception e, LogLevel level, string file, string member, int line);
    }
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Exception = 5,
        Disabled = 6
    }
}
