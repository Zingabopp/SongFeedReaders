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
    public class BeastSaberFollowsFeed : BeastSaberFeed, IPagedFeed
    {
        /// <inheritdoc/>
        public BeastSaberFollowsFeed(BeastSaberFollowsSettings feedSettings, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public BeastSaberFollowsFeed(ISettingsFactory settingsFactory, IBeastSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.Follows";

        /// <inheritdoc/>
        public override string DisplayName => "BeastSaber Follows";

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
            BeastSaberFollowsSettings s = (BeastSaberFollowsSettings)FeedSettings;
            return new Uri(BaseUri, $"https://bsaber.com/wp-json/bsaber-api/songs/?followed_by={s.Username}&count=50&page={page}");
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            IFeedSettings feedSettings = FeedSettings;
            if (feedSettings == null)
                throw new InvalidFeedSettingsException("Feed Settings is null.");

            if (!(FeedSettings is BeastSaberFollowsSettings bSettings))
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
            if (settings is BeastSaberFollowsSettings bSettings)
            {
                return !string.IsNullOrWhiteSpace(bSettings.Username);
            }
            return false;
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            EnsureValidSettings();
            BeastSaberFollowsSettings s = (BeastSaberFollowsSettings)settings;
            return new PagedFeedAsyncEnumerator(this, s.StartingPage, 
                FeedStartingPage, Logger);
        }
    }
}
