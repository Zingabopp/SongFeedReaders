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
    public class ScoreSaberTrendingFeed : ScoreSaberPagedFeed<ScoreSaberTrendingSettings>
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTrendingFeed"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTrendingFeed(IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
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
    public class ScoreSaberLatestFeed : ScoreSaberPagedFeed<ScoreSaberLatestSettings>
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberLatestFeed"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberLatestFeed(IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
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
    public class ScoreSaberTopPlayedFeed : ScoreSaberPagedFeed<ScoreSaberTopPlayedSettings>
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopPlayedFeed"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopPlayedFeed(IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
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
    public class ScoreSaberTopRankedFeed : ScoreSaberPagedFeed<ScoreSaberTopRankedSettings>
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberTopRankedFeed"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public ScoreSaberTopRankedFeed(IScoreSaberPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
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
