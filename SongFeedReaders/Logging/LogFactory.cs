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
        private readonly Func<ILoggerSettings, string?, ILogger> _factory;
        private readonly ILoggerSettings _settings;
        /// <summary>
        /// Creates a new <see cref="LogFactory"/>.
        /// </summary>
        /// <param name="logFactory"></param>
        /// <param name="loggerSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogFactory(Func<ILoggerSettings, string?, ILogger> logFactory, ILoggerSettings loggerSettings)
        {
            _factory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            _settings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
        }

        /// <inheritdoc/>
        public ILogger GetLogger(string? moduleName = null)
        {
            if (moduleName != null)
            {
                return _factory(_settings, moduleName);
            }

            StackFrame frame = new StackFrame(1, false);
            MethodBase method = frame.GetMethod();
            Type? declaringType = method?.DeclaringType;
            moduleName = declaringType == null 
                ? method!.Name 
                : declaringType.Name;
            return _factory(_settings, moduleName);
        }

        /// <inheritdoc/>
        public ILogger GetLogger<T>()
            => GetLogger(typeof(T).Name);
    }
}
