using System;
using SongFeedReaders.Attributes;
using SongFeedReaders.Logging;
using SongFeedReaders.Utilities;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// This feed returns curator recommended songs from BeatSaver
    /// </summary>
    [Feed(typeof(BeatSaverCuratorSettings))]
    public class BeatSaverCuratorFeed : BeatSaverFeed<BeatSaverCuratorSettings>, IDatedFeed
    {
        
        /// <summary>
        /// Initializes a new <see cref="BeatSaverCuratorFeed"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeatSaverCuratorFeed(IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(pageHandler, webClient, logFactory)
        {

        }
        
        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.CuratorRecommended";

        /// <inheritdoc/>
        public override string DisplayName => "Beat Saver Curator Recommended";

        /// <inheritdoc/>
        public override string Description => "Curator recommended maps from BeatSaver";

        /// <inheritdoc/>
        public Uri GetUriForDate(FeedDate feedDate)
        {
            EnsureValidSettings();
            if (feedDate.Direction == DateDirection.Before)
                return new Uri(BeatSaverHelper.BeatSaverApiUri, $"maps/latest?automapper=false&sort=CURATED&before={feedDate.Date.ToUTCWebTime()}");
            else
                return new Uri(BeatSaverHelper.BeatSaverApiUri, $"maps/latest?automapper=false&sort=CURATED&after={feedDate.Date.ToUTCWebTime()}");
        }

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings? settings)
        {
            if (!(settings is BeatSaverLatestSettings castSettings))
            {
                return false;

            }
            if (castSettings.StartingDate > castSettings.EndingDate)
            {
                return false;
            }
            return true;
        }
        
        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            if (FeedSettings == null)
                throw new InvalidFeedSettingsException("FeedSettings is null.");
            if (FeedSettings.StartingDate > FeedSettings.EndingDate)
            {
                throw new InvalidFeedSettingsException($"StartingDate cannot be greater than the EndingDate.");
            }
        }
        
        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator()
        {
            EnsureValidSettings();
            return new DatedFeedAsyncEnumerator(this, false, Logger);
        }
    }
}