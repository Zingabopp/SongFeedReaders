using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// A feed that reads songs uploaded by mappers a bsaber.com user is following.
    /// </summary>
    public class BeastSaberCuratorFeed : BeastSaberFeed, IPagedFeed
    {
        /// <inheritdoc/>
        public BeastSaberCuratorFeed(BeastSaberCuratorSettings feedSettings, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public BeastSaberCuratorFeed(ISettingsFactory settingsFactory, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.CuratorRecommended";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Curator Recommended";

        /// <inheritdoc/>
        public override string Description => "Songs uploaded by mappers a user is following.";
        /// <summary>
        /// BeastSaber API pages start at 1.
        /// </summary>
        protected readonly int FeedStartingPage = 1;

        /// <inheritdoc/>
        public Uri GetUriForPage(int page)
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
            if (!(FeedSettings is BeastSaberCuratorSettings settings))
            {
                throw new InvalidFeedSettingsException($"Settings is the wrong type ({FeedSettings?.GetType().Name}), "
                    + $"should be {nameof(BeastSaberCuratorSettings)}");

            }
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            EnsureValidSettings();
            BeastSaberCuratorSettings s = (BeastSaberCuratorSettings)settings;
            return new PagedFeedAsyncEnumerator(this, s.StartingPage, FeedStartingPage,
                Logger);
        }
    }
}
