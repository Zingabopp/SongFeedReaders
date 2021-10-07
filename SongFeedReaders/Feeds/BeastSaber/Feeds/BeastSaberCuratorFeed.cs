using SongFeedReaders.Attributes;
using SongFeedReaders.Logging;
using System;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// A feed that reads songs uploaded by mappers a bsaber.com user is following.
    /// </summary>
    [Feed(typeof(BeastSaberCuratorSettings))]
    public class BeastSaberCuratorFeed : BeastSaberPagedFeed<BeastSaberCuratorSettings>
    {
        /// <inheritdoc/>
        public BeastSaberCuratorFeed(IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.CuratorRecommended";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Curator Recommended";

        /// <inheritdoc/>
        public override string Description => "Songs uploaded by mappers a user is following.";

        /// <inheritdoc/>
        public override Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            return new Uri(BaseUri, $"wp-json/bsaber-api/songs/?bookmarked_by=curatorrecommended&page={page}");
        }

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (settings is BeastSaberCuratorSettings)
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            if (FeedSettings == null)
                throw new InvalidFeedSettingsException("FeedSettings is null.");
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator()
        {
            EnsureValidSettings();
            return new PagedFeedAsyncEnumerator(this, Logger);
        }
    }
}
