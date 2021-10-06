﻿using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// This feed returns the latest songs uploaded to Beat Saver.
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
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="BeatSaverLatestFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeatSaverLatestFeed(ISettingsFactory settingsFactory, IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {

        }

        /// <inheritdoc/>
        public override string FeedId => $"{ServiceId}.Latest";

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
            if (!(FeedSettings is BeatSaverLatestSettings settings))
            {
                throw new InvalidFeedSettingsException($"Settings is the wrong type ({FeedSettings?.GetType().Name}), "
                    + $"should be {nameof(BeatSaverLatestSettings)}");
            }
            if (settings.StartingDate > settings.EndingDate)
            {
                throw new InvalidFeedSettingsException($"StartingDate cannot be greater than the EndingDate.");
            }
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            if (!AreSettingsValid(settings))
                throw new InvalidFeedSettingsException();
            return new DatedFeedAsyncEnumerator(this, false, Logger);
        }

    }
}
