﻿using SongFeedReaders.Logging;
using System;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// ScoreSaber feed number. Replaces <see cref="ScoreSaberFeed{TFeedSettings}.CATKEY"/>
    /// in <see cref="ScoreSaberFeed{TFeedSettings}.URL_TEMPLATE"/>.
    /// </summary>
    public enum FeedNumber
    {
        /// <summary>
        /// Trending feed.
        /// </summary>
        Trending = 0,
        /// <summary>
        /// Latest ranked feed.
        /// </summary>
        LatestRanked = 1,
        /// <summary>
        /// Top played feed.
        /// </summary>
        TopPlayed = 2,
        /// <summary>
        /// Top ranked feed.
        /// </summary>
        TopRanked = 3
    }

    /// <summary>
    /// Base class for ScoreSaber feeds.
    /// </summary>
    public abstract class ScoreSaberFeed<TFeedSettings> : FeedBase<TFeedSettings>
        where TFeedSettings : ScoreSaberFeedSettings
    {
        /// <summary>
        /// Replace with page number in <see cref="URL_TEMPLATE"/>.
        /// </summary>
        protected static readonly string PAGENUMKEY = "{PAGENUM}";
        /// <summary>
        /// Replace with a <see cref="FeedNumber"/> in <see cref="URL_TEMPLATE"/>.
        /// </summary>
        protected static readonly string CATKEY = "{CAT}";
        /// <summary>
        /// Replace with '0' to show all songs or '1' to show only ranked songs in <see cref="URL_TEMPLATE"/>.
        /// </summary>
        protected static readonly string RANKEDKEY = "{RANKKEY}";
        /// <summary>
        /// Replace with number of songs to return per page in <see cref="URL_TEMPLATE"/>.
        /// </summary>
        protected static readonly string LIMITKEY = "{LIMIT}";

        /// <summary>
        /// URL template to append on <see cref="BaseUri"/>.
        /// Must string replace <see cref="CATKEY"/>, <see cref="PAGENUMKEY"/>, <see cref="RANKEDKEY"/>, 
        /// and <see cref="LIMITKEY"/> with the appropriate values.
        /// </summary>
        protected static readonly string URL_TEMPLATE
            = $"api.php?function=get-leaderboards&cat={CATKEY}&limit={LIMITKEY}&page={PAGENUMKEY}&ranked={RANKEDKEY}";


        private const string FORMATTABLE_URL_TEMPLATE
            = "api.php?function=get-leaderboards&cat={0}&limit={1}&page={2}&ranked={3}";

        /// <summary>
        /// Creates a ScoreSaber feed <see cref="Uri"/> for the given inputs.
        /// </summary>
        /// <param name="feedNumber"></param>
        /// <param name="pageNum"></param>
        /// <param name="rankedOnly"></param>
        /// <param name="songsPerPage"></param>
        /// <returns></returns>
        public static Uri CreateUri(FeedNumber feedNumber, int pageNum, bool rankedOnly, int songsPerPage)
        {
            string url = string.Format(FORMATTABLE_URL_TEMPLATE,
                (int)feedNumber,
                songsPerPage,
                pageNum,
                rankedOnly ? '1' : '0');
            return new Uri(BaseUri, url);
        }


        /// <inheritdoc/>
        public override string ServiceId => "ScoreSaber";
        /// <summary>
        /// Base URI for bsaber.com
        /// </summary>
        protected static readonly Uri BaseUri = new Uri("https://scoresaber.com/");

        /// <summary>
        /// The <see cref="FeedNumber"/> associated with this <see cref="ScoreSaberFeed{TFeedSettings}"/>.
        /// </summary>
        protected abstract FeedNumber Feed { get; }
        /// <summary>
        /// Initializes a new <see cref="ScoreSaberFeed{TFeedSettings}"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberFeed(IScoreSaberPageHandler pageHandler,
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

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (!(settings is ScoreSaberFeedSettings))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            if (FeedSettings == null)
                throw new InvalidFeedSettingsException("FeedSettings is null.");
        }
    }
    /// <summary>
    /// Base class for ScoreSaber paged feeds.
    /// </summary>
    /// <typeparam name="TFeedSettings"></typeparam>
    public abstract class ScoreSaberPagedFeed<TFeedSettings> : ScoreSaberFeed<TFeedSettings>, IPagedFeed
        where TFeedSettings : ScoreSaberFeedSettings, IPagedFeedSettings
    {
        /// <inheritdoc/>
        public ScoreSaberPagedFeed(IScoreSaberPageHandler pageHandler,
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
        public virtual Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            ScoreSaberFeedSettings settings = FeedSettings
                ?? throw new InvalidFeedSettingsException(InvalidFeedSettingsException.NullMessage);
            return CreateUri(Feed, page, settings.RankedOnly, settings.SongsPerPage);
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator()
        {
            EnsureValidSettings();
            return new PagedFeedAsyncEnumerator(this, Logger);
        }
    }
}
