using System;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Settings for Beat Saver Curator Recommended feed
    /// </summary>
    public class BeatSaverCuratorSettings : BeatSaverFeedSettings, IDatedFeedSettings
    {

        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.CuratorRecommended";
        
        /// <inheritdoc/>
        public override int FeedIndex => 0;

        /// <inheritdoc/>
        public override int SongsPerPage => 20;
        
        /// <inheritdoc/>
        public DateTime StartingDate { get; set; } = DateTime.MinValue;
        
        /// <inheritdoc/>
        public DateTime EndingDate { get; set; } = DateTime.MaxValue;

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeatSaverMapperSettings()
            {
                MaxSongs = this.MaxSongs,
                Filter = this.Filter,
                StopWhenAny = this.StopWhenAny,
                StoreRawData = this.StoreRawData
            };
        }
    }
}