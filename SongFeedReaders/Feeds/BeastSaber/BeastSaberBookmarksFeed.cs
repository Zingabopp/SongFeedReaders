using SongFeedReaders.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// A feed that reads songs bookmarked by a bsaber.com user.
    /// </summary>
    public class BeastSaberBookmarksFeed : BeastSaberFeed, IPagedFeed
    {
        /// <inheritdoc/>
        public BeastSaberBookmarksFeed(BeastSaberBookmarksSettings feedSettings, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.Bookmarks";

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
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (settings is BeastSaberBookmarksSettings bSettings)
            {
                return !string.IsNullOrWhiteSpace(bSettings.Username);
            }
            return false;
        }

        /// <inheritdoc/>
        protected override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            EnsureValidSettings();
            BeastSaberBookmarksSettings s = (BeastSaberBookmarksSettings)settings;
            return new PagedFeedAsyncEnumerator(this, s.StartingPage, 
                FeedStartingPage, Logger);
        }
    }
}
