using SongFeedReaders.Attributes;
using SongFeedReaders.Logging;
using System;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// A feed that reads songs uploaded by mappers a bsaber.com user is following.
    /// </summary>
    [Feed(typeof(BeastSaberFollowsSettings))]
    public class BeastSaberFollowsFeed : BeastSaberPagedFeed<BeastSaberFollowsSettings>
    {
        /// <inheritdoc/>
        public BeastSaberFollowsFeed(IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.Follows";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Follows";

        /// <inheritdoc/>
        public override string Description => "Songs uploaded by mappers a user is following.";

        /// <inheritdoc/>
        public override Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            BeastSaberFollowsSettings s = FeedSettings!;
            return new Uri(BaseUri, $"https://bsaber.com/wp-json/bsaber-api/songs/?followed_by={s.Username}&count=50&page={page}");
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            BeastSaberFollowsSettings feedSettings = FeedSettings!;
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
            if (settings is BeastSaberFollowsSettings bSettings)
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
