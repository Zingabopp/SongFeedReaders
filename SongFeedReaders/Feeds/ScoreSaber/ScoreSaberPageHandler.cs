using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// Handles parsing the page content from ScoreSaber.
    /// </summary>
    public class ScoreSaberPageHandler : FeedPageHandlerBase, IScoreSaberPageHandler
    {
        /// <inheritdoc/>
        public override PageReadResult Parse(PageContent content, Uri? pageUri, IFeedSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
