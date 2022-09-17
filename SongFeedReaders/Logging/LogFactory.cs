using System;
using System.Diagnostics;
using System.Reflection;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// A basic <see cref="ILogFactory"/>.
    /// </summary>
    public sealed class LogFactory : ILogFactory
    {
        private readonly Func<ILoggerSettings, string?, ILogger> Factory;
        private readonly ILoggerSettings _settings;
        /// <summary>
        /// Creates a new <see cref="LogFactory"/>.
        /// </summary>
        /// <param name="logFactory"></param>
        /// <param name="loggerSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogFactory(Func<ILoggerSettings, string?, ILogger> logFactory, ILoggerSettings loggerSettings)
        {
            Factory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            _settings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
        }

        /// <inheritdoc/>
        public ILogger GetLogger(string? moduleName = null)
        {
            if (moduleName == null)
            {
                StackFrame frame = new StackFrame(1, false);
                MethodBase method = frame.GetMethod();
                Type declaringType = method.DeclaringType;
                if (declaringType == null)
                    moduleName = method.Name;
                else
                    moduleName = declaringType.Name;
            }
            return Factory(_settings, moduleName);
        }
    }
}
