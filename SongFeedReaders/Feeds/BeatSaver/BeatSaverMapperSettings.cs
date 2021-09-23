namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Settings for the Beat Saver Latest feed.
    /// </summary>
    public class BeatSaverMapperSettings : BeatSaverFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Mapper";

        /// <inheritdoc/>
        public override int FeedIndex => 0;

        /// <inheritdoc/>
        public override int SongsPerPage => 20;
        /// <summary>
        /// Beat Saver username of the mapper.
        /// </summary>
        public string MapperName { get; set; } = string.Empty;

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
