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
    public class BeastSaberBookmarksFeed : BeastSaberPagedFeed<BeastSaberBookmarksSettings>
    {
        /// <inheritdoc/>
        public BeastSaberBookmarksFeed(IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.Bookmarks";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Bookmarks";

        /// <inheritdoc/>
        public override string Description => "Songs bookmarked by a user.";

        /// <inheritdoc/>
        public override Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            BeastSaberBookmarksSettings s = FeedSettings!;
            return new Uri(BaseUri, $"wp-json/bsaber-api/songs/?bookmarked_by={s.Username}&page={page}");
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            BeastSaberBookmarksSettings? feedSettings = FeedSettings;
            if (feedSettings == null)
                throw new InvalidFeedSettingsException("Feed Settings is null.");
            if (string.IsNullOrWhiteSpace(feedSettings.Username))
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
        public override FeedAsyncEnumerator GetAsyncEnumerator()
        {
            EnsureValidSettings();
            return new PagedFeedAsyncEnumerator(this, Logger);
        }
    }
}
