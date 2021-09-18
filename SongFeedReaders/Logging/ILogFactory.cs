using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger();
        ILogger GetLogger(string moduleName);
    }
}
