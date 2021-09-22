using SongFeedReaders.Logging;
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
            return new Uri(BaseUri, $"members/{s.Username}/wall/followings/feed/?acpage={page}");
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
        protected override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            EnsureValidSettings();
            BeastSaberFollowsSettings s = (BeastSaberFollowsSettings)settings;
            return new PagedFeedAsyncEnumerator(this, s.StartingPage, FeedStartingPage);
        }
    }
}
