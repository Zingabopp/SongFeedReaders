using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Settings for the Beast Saber Follows feed.
    /// </summary>
    public class BeastSaberFollowsSettings : BeastSaberFeedSettings
    {

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.Follows";

        /// <inheritdoc/>
        public override int FeedIndex => 1;
        /// <inheritdoc/>
        public override int SongsPerPage => 100; // TODO: Not correct, should this even be here?

        /// <summary>
        /// bsaber.com username.
        /// </summary>
        public string? Username { get; set; }

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeastSaberFollowsSettings()
            {
                MaxSongs = this.MaxSongs,
                Filter = this.Filter,
                StopWhenAny = this.StopWhenAny,
                StoreRawData = this.StoreRawData,
                Username = this.Username
            };
        }
    }
}
