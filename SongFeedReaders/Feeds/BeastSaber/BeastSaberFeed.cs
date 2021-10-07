using SongFeedReaders.Logging;
using System;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Base class for Beast Saber feeds.
    /// </summary>
    public abstract class BeastSaberFeed<TFeedSettings> : FeedBase<TFeedSettings>
        where TFeedSettings : class, IFeedSettings
    {
        private const string MIME_XML = "text/xml";
        private const string MIME_JSON = "application/json";
        /// <inheritdoc/>
        public override string ServiceId => "BeastSaber";
        /// <summary>
        /// Base URI for bsaber.com
        /// </summary>
        protected static readonly Uri BaseUri = new Uri("https://bsaber.com/");

        /// <summary>
        /// Initializes a new <see cref="BeastSaberFeed{TFeedSettings}"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeastSaberFeed(IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
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
    /// <summary>
    /// Base class for BeastSaber paged feeds.
    /// </summary>
    /// <typeparam name="TFeedSettings"></typeparam>
    public abstract class BeastSaberPagedFeed<TFeedSettings> : BeastSaberFeed<TFeedSettings>, IPagedFeed
        where TFeedSettings : BeastSaberFeedSettings, IPagedFeedSettings
    {
        /// <inheritdoc/>
        public BeastSaberPagedFeed(IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        IPagedFeedSettings? IPagedFeed.GetPagedFeedSettings() => FeedSettings;
        /// <summary>
        /// BeastSaber API pages start at 1.
        /// </summary>
        public virtual int FeedStartingPage => 1;

        /// <inheritdoc/>
        public abstract Uri GetUriForPage(int page);
    }
}
