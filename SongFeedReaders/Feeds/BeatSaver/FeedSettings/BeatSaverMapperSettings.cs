using System;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Settings for the Beat Saver Latest feed.
    /// </summary>
    public class BeatSaverMapperSettings : BeatSaverFeedSettings, IPagedFeedSettings
    {
        private int _startingPage;

        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Mapper";

        /// <inheritdoc/>
        public override int FeedIndex => 2;

        /// <inheritdoc/>
        public override int SongsPerPage => 20;
        /// <summary>
        /// Beat Saver username of the mapper.
        /// </summary>
        public string MapperName { get; set; } = string.Empty;

        /// <summary>
        /// Page of the feed to start on, default is 1. Setting '1' here is the same as starting on the first page.
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when set to less than 1.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to less than 1.</exception>
        public int StartingPage
        {
            get { return _startingPage; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(StartingPage), "StartingPage cannot be less than 1.");
                _startingPage = value;
            }
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeatSaverMapperSettings()
            {
                MapperName = this.MapperName,
                MaxSongs = this.MaxSongs,
                Filter = this.Filter,
                StopWhenAny = this.StopWhenAny,
                StoreRawData = this.StoreRawData
            };
        }
    }
}
