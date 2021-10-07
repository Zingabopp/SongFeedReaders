using SongFeedReaders.Models;
using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Base class for basic feed settings.
    /// </summary>
    public abstract class FeedSettingsBase : IFeedSettings
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

        /// <summary>
        /// Copies the values from this instance to <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        protected virtual void CopyTo<T>(T target) where T : FeedSettingsBase
        {
            target.MaxSongs = MaxSongs;
            target.StoreRawData = StoreRawData;
            target.Filter = Filter;
            target.StopWhenAny = StopWhenAny;
        }
    }
}
