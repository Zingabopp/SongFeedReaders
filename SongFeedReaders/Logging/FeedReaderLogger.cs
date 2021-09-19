using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReaders.Logging
{
    public class FeedReaderLogger
        : FeedReaderLoggerBase
    {
        public FeedReaderLogger()
            : base()
        {

        }

        public FeedReaderLogger(ILoggerSettings loggerSettings)
            : base(loggerSettings)
        {
        }

        public override void Log(string message, LogLevel logLevel, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
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
                timePart = $" @ {DateTime.Now.ToString("HH:mm")}";
            Console.WriteLine($"{sourcePart}{timePart} - {logLevel}] {message}");
        }

        public override void Log(Exception e, LogLevel logLevel, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
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
                timePart = $" @ {DateTime.Now.ToString("HH:mm")}";
            Console.WriteLine($"{sourcePart}{timePart} - {logLevel}] {e.Message}");
            Console.WriteLine(e);
        }
    }
}
