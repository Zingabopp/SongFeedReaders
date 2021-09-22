using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Settings for the Beast Saber Curator Recommended feed.
    /// </summary>
    public class BeastSaberCuratorSettings : BeastSaberFeedSettings
    {

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.CuratorRecommended";

        /// <inheritdoc/>
        public override int FeedIndex => 2;
        /// <inheritdoc/>
        public override int SongsPerPage => 100; // TODO: Not correct, should this even be here?

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeastSaberCuratorSettings()
            {
                MaxSongs = this.MaxSongs,
                Filter = this.Filter,
                StopWhenAny = this.StopWhenAny,
                StoreRawData = this.StoreRawData
            };
        }
    }
}
