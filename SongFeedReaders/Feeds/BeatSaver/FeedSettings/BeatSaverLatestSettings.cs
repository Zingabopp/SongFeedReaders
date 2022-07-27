using System;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Settings for the Beat Saver Latest feed.
    /// </summary>
    public class BeatSaverLatestSettings : BeatSaverFeedSettings, IDatedFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Latest";

        /// <inheritdoc/>
        public override int FeedIndex => 1;

        /// <inheritdoc/>
        public override int SongsPerPage => 20;

        /// <inheritdoc/>
        public DateTime StartingDate { get; set; } = DateTime.MinValue;
        /// <inheritdoc/>
        public DateTime EndingDate { get; set; } = DateTime.MaxValue;

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeatSaverLatestSettings()
            {
                MaxSongs = this.MaxSongs,
                Filter = this.Filter,
                StopWhenAny = this.StopWhenAny,
                StoreRawData = this.StoreRawData
            };
        }
    }
}
