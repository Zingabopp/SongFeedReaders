using SongFeedReaders.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReadersTests
{
    internal static class Utilities
    {
        public static ILogFactory DefaultLogFactory => new LogFactory(logFactory);

        private static ILogger logFactory(string? moduleName)
        {
            return new TestConsoleLogger(moduleName);
        }

        private class TestConsoleLogger : ILogger
        {
            private string? ModuleName;
            public TestConsoleLogger(string? moduleName)
            {
                ModuleName = moduleName;
            }

            public void Log(string message, LogLevel level, 
                [CallerFilePath] string? file = null, 
                [CallerMemberName] string? member = null, 
                [CallerLineNumber] int line = 0)
            {
                Console.WriteLine(GetPrefix(level, file, member, line) + message);
            }

            public void Log(Exception e, LogLevel level, 
                [CallerFilePath] string? file = null, 
                [CallerMemberName] string? member = null, 
                [CallerLineNumber] int line = 0)
            {
                Console.WriteLine(GetPrefix(level, file, member, line) + e.ToString());
            }

            private string GetPrefix(LogLevel level, string? file, string? member, int line)
            {
                return $"[{level} {ModuleName}->{member}:{line}]: ";
            }
        }
    }
}
