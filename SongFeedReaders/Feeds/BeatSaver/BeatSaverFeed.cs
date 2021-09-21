using SongFeedReaders.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public abstract class BeatSaverFeed : FeedBase
    {
        /// <summary>
        /// Initializes a new <see cref="BeatSaverFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="logFactory"></param>
        protected BeatSaverFeed(IFeedSettings feedSettings, IBeatSaverPageHandler pageHandler, 
            ILogFactory? logFactory)
            : base(feedSettings, pageHandler, logFactory)
        {

        }
    }
}
