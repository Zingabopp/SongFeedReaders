using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Base class for Beast Saber feeds.
    /// </summary>
    public abstract class BeastSaberFeed : FeedBase
    {
        private const string MIME_XML = "text/xml";
        private const string MIME_JSON = "application/json";
        /// <summary>
        /// Base URI for bsaber.com
        /// </summary>
        protected static readonly Uri BaseUri = new Uri("https://bsaber.com/");

        /// <summary>
        /// Initializes a new <see cref="BeastSaberFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeastSaberFeed(BeastSaberFeedSettings feedSettings, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="BeastSaberFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeastSaberFeed(ISettingsFactory settingsFactory, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        protected override async Task<PageContent> GetPageContent(IWebResponseContent responseContent)
        {
            string pageText = await responseContent.ReadAsStringAsync().ConfigureAwait(false);
            string? contentTypeStr = responseContent.ContentType?.ToLower();
            string contentId = PageContent.ContentId_Unknown;
            if (contentTypeStr != null)
            {
                if (contentTypeStr == MIME_JSON)
                    contentId = PageContent.ContentId_JSON;
                else if (contentTypeStr == MIME_XML)
                    contentId = PageContent.ContentId_XML;
            }
            return new PageContent(contentId, pageText);
        }
    }
}
