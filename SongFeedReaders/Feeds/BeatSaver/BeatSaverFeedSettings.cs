using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feed settings.
    /// </summary>
    public abstract class BeatSaverFeedSettings : IFeedSettings
    {
        /// <inheritdoc/>
        public abstract string FeedId { get; }
        /// <inheritdoc/>
        public abstract int FeedIndex { get; }
        /// <inheritdoc/>
        public abstract int SongsPerPage { get; }
        /// <inheritdoc/>
        public int MaxSongs { get; set; }
        /// <inheritdoc/>
        public bool StoreRawData { get; set; }
        /// <inheritdoc/>
        public Func<ScrapedSong, bool>? Filter { get; set; }
        /// <inheritdoc/>
        public Func<ScrapedSong, bool>? StopWhenAny { get; set; }

        /// <inheritdoc/>
        public abstract object Clone();
    }
}
