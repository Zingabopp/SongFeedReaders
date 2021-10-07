using SongFeedReaders.Logging;
using System;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public abstract class BeatSaverFeed<TFeedSettings> : FeedBase<TFeedSettings>
        where TFeedSettings : class, IFeedSettings
    {
        /// <inheritdoc/>
        public override string ServiceId => "BeatSaver";

        /// <summary>
        /// Initializes a new <see cref="BeatSaverFeed{TFeedSettings}"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        protected BeatSaverFeed(IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        protected override async Task<PageContent> GetPageContent(IWebResponseContent responseContent)
        {
            string pageText = await responseContent.ReadAsStringAsync().ConfigureAwait(false);
            return new PageContent(PageContent.ContentId_JSON, pageText);
        }
    }
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public abstract class BeatSaverPagedFeed<TFeedSettings> : BeatSaverFeed<TFeedSettings>, IPagedFeed
        where TFeedSettings : class, IFeedSettings, IPagedFeedSettings
    {
        /// <inheritdoc/>
        protected BeatSaverPagedFeed(IBeatSaverPageHandler pageHandler,
               IWebClient webClient, ILogFactory? logFactory = null)
               : base(pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        IPagedFeedSettings? IPagedFeed.GetPagedFeedSettings() => FeedSettings;
        /// <inheritdoc/>
        public abstract int FeedStartingPage { get; }
        /// <inheritdoc/>
        public abstract Uri GetUriForPage(int page);
    }

}
