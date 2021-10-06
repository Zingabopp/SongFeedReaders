using SongFeedReaders.Attributes;
using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// A feed that reads songs bookmarked by a bsaber.com user.
    /// </summary>
    [Feed(typeof(BeastSaberBookmarksSettings))]
    public class BeastSaberBookmarksFeed : BeastSaberFeed, IPagedFeed
    {
        /// <inheritdoc/>
        public BeastSaberBookmarksFeed(BeastSaberBookmarksSettings feedSettings, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public BeastSaberBookmarksFeed(ISettingsFactory settingsFactory, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.Bookmarks";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Bookmarks";

        /// <inheritdoc/>
        public override string Description => "Songs bookmarked by a user.";
        /// <summary>
        /// BeastSaber API pages start at 1.
        /// </summary>
        protected readonly int FeedStartingPage = 1;

        /// <inheritdoc/>
        public Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            BeastSaberBookmarksSettings s = (BeastSaberBookmarksSettings)FeedSettings;
            return new Uri(BaseUri, $"wp-json/bsaber-api/songs/?bookmarked_by={s.Username}&page={page}");
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            IFeedSettings feedSettings = FeedSettings;
            if (feedSettings == null)
                throw new InvalidFeedSettingsException("Feed Settings is null.");

            if (!(FeedSettings is BeastSaberBookmarksSettings bSettings))
            {
                throw new InvalidFeedSettingsException($"Feed settings '{feedSettings.GetType().Name}' is the wrong type for {GetType().Name}");
            }
            if (string.IsNullOrWhiteSpace(bSettings.Username))
            {
                throw new InvalidFeedSettingsException("No Beast Saber username specified in settings.");
            }
        }

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (settings is BeastSaberBookmarksSettings bSettings)
            {
                return !string.IsNullOrWhiteSpace(bSettings.Username);
            }
            return false;
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            EnsureValidSettings();
            BeastSaberBookmarksSettings s = (BeastSaberBookmarksSettings)settings;
            return new PagedFeedAsyncEnumerator(this, s.StartingPage, 
                FeedStartingPage, Logger);
        }
    }
}
