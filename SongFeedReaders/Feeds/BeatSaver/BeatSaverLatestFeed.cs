using SongFeedReaders.Logging;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public class BeatSaverLatestFeed : BeatSaverFeed, IDatedFeed
    {
        /// <summary>
        /// Initializes a new <see cref="BeatSaverLatestFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeatSaverLatestFeed(BeatSaverLatestSettings feedSettings, IBeatSaverPageHandler pageHandler, 
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {

        }

        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Latest";

        /// <inheritdoc/>
        public override string DisplayName => "Beat Saver Latest";

        /// <inheritdoc/>
        public override string Description => "The latest songs uploaded to Beat Saver";

        /// <inheritdoc/>
        public Uri GetUriForDate(FeedDate feedDate)
        {
            EnsureValidSettings();
            if (feedDate.Direction == DateDirection.Before)
                return new Uri(BeatSaverHelper.BeatSaverApiUri, $"maps/latest?automapper=false&before={feedDate.Date.ToUTCWebTime()}");
            else
                return new Uri(BeatSaverHelper.BeatSaverApiUri, $"maps/latest?automapper=false&after={feedDate.Date.ToUTCWebTime()}");
        }

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (!(settings is BeatSaverLatestSettings latest))
                return false;

            return true;
        }

        /// <inheritdoc/>
        protected override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            if (!AreSettingsValid(settings))
                throw new InvalidFeedSettingsException();
            return new DatedFeedAsyncEnumerator(this, false);
        }
    }
}
