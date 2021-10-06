using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// This feed returns the songs that are trending on ScoreSaber.
    /// </summary>
    public class ScoreSaberTrendingFeed : ScoreSaberFeed, IPagedFeed
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTrendingFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTrendingFeed(ScoreSaberTrendingSettings feedSettings, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTrendingFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTrendingFeed(ISettingsFactory settingsFactory, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.Trending";

        /// <inheritdoc/>
        public override string DisplayName => "ScoreSaber Trending";

        /// <inheritdoc/>
        public override string Description => "Songs that are trending on ScoreSaber.";

        /// <inheritdoc/>
        protected override FeedNumber Feed => FeedNumber.Trending;
    }


    /// <summary>
    /// This feed returns the songs that were recently ranked on ScoreSaber.
    /// </summary>
    public class ScoreSaberLatestFeed : ScoreSaberFeed, IPagedFeed
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberLatestFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberLatestFeed(ScoreSaberLatestSettings feedSettings, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ScoreSaberLatestFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberLatestFeed(ISettingsFactory settingsFactory, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.LatestRanked";

        /// <inheritdoc/>
        public override string DisplayName => "ScoreSaber Latest Ranked";

        /// <inheritdoc/>
        public override string Description => "Songs that were recently ranked on ScoreSaber.";

        /// <inheritdoc/>
        protected override FeedNumber Feed => FeedNumber.LatestRanked;
    }


    /// <summary>
    /// This feed returns the songs that are the most played on ScoreSaber.
    /// </summary>
    public class ScoreSaberTopPlayedFeed : ScoreSaberFeed, IPagedFeed
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopPlayedFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopPlayedFeed(ScoreSaberTopPlayedSettings feedSettings, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopPlayedFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopPlayedFeed(ISettingsFactory settingsFactory, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.TopPlayed";

        /// <inheritdoc/>
        public override string DisplayName => "ScoreSaber Top Played";

        /// <inheritdoc/>
        public override string Description => "Songs that are the most played on ScoreSaber.";

        /// <inheritdoc/>
        protected override FeedNumber Feed => FeedNumber.TopPlayed;
    }


    /// <summary>
    /// This feed returns the songs that are highest ranked on ScoreSaber.
    /// </summary>
    public class ScoreSaberTopRankedFeed : ScoreSaberFeed, IPagedFeed
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopRankedFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopRankedFeed(ScoreSaberTopRankedSettings feedSettings, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopRankedFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopRankedFeed(ISettingsFactory settingsFactory, IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.TopRanked";

        /// <inheritdoc/>
        public override string DisplayName => "ScoreSaber Top Ranked";

        /// <inheritdoc/>
        public override string Description => "Songs that are highest ranked on ScoreSaber.";

        /// <inheritdoc/>
        protected override FeedNumber Feed => FeedNumber.TopRanked;
    }
}
