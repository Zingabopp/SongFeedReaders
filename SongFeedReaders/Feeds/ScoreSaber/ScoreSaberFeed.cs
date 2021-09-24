using SongFeedReaders.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// Base class for ScoreSaber feeds.
    /// </summary>
    public abstract class ScoreSaberFeed : FeedBase
    {
        /// <summary>
        /// Base URI for bsaber.com
        /// </summary>
        protected static readonly Uri BaseUri = new Uri("https://bsaber.com/");

        /// <summary>
        /// Initializes a new <see cref="ScoreSaberFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberFeed(ScoreSaberFeedSettings feedSettings, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        protected override async Task<PageContent> GetPageContent(IWebResponseContent responseContent)
        {
            string pageText = await responseContent.ReadAsStringAsync().ConfigureAwait(false);
            return new PageContent(PageContent.ContentId_JSON, pageText);
        }
    }
}
