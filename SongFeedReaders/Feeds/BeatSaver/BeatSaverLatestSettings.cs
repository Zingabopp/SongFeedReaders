namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Settings for the Beat Saver Latest feed.
    /// </summary>
    public class BeatSaverLatestSettings : BeatSaverFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Latest";

        /// <inheritdoc/>
        public override int FeedIndex => 0;

        /// <inheritdoc/>
        public override int SongsPerPage => 50;

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
