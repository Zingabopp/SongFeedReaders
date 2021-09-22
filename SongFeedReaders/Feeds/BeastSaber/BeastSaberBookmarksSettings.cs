using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Settings for the Beast Saber Bookmarks feed.
    /// </summary>
    public class BeastSaberBookmarksSettings : BeastSaberFeedSettings
    {

        /// <inheritdoc/>
        public override string FeedId => "BeastSaber.Bookmarks";

        /// <inheritdoc/>
        public override int FeedIndex => 0;
        /// <inheritdoc/>
        public override int SongsPerPage => 100; // TODO: Not correct, should this even be here?

        /// <summary>
        /// bsaber.com username.
        /// </summary>
        public string? Username { get; set; }

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BeastSaberBookmarksSettings()
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
