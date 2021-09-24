using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SongFeedReaders.Logging
{
    /// <summary>
    /// A basic <see cref="ILogFactory"/>.
    /// </summary>
    public sealed class LogFactory : ILogFactory
    {
        private readonly Func<string?, ILogger> Factory;
        /// <summary>
        /// Creates a new <see cref="LogFactory"/>.
        /// </summary>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogFactory(Func<string?, ILogger> logFactory)
        {
            Factory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
        }

        /// <inheritdoc/>
        public ILogger GetLogger(string? moduleName = null)
        {
            if(moduleName == null)
            {
                StackFrame frame = new StackFrame(1, false);
                MethodBase method = frame.GetMethod();
                Type declaringType = method.DeclaringType;
                if (declaringType == null)
                    moduleName = method.Name;
                else
                    moduleName = declaringType.Name;
            }
            return Factory(moduleName);
        }
    }
}
