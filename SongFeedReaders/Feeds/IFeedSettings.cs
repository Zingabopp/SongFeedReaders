using SongFeedReaders.Models;
using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Settings for a feed.
    /// </summary>
    public interface IFeedSettings : ICloneable
    {
        /// <summary>
        /// Unique string ID of the feed. (Format should be 'Service.FeedName').
        /// Should match the feed that uses it. TODO: Not using this anymore?
        /// </summary>
        string FeedId { get; }
        /// <summary>
        /// Index of the feed. TODO: Not using this anymore?
        /// </summary>
        int FeedIndex { get; }

        /// <summary>
        /// Number of songs per page.
        /// </summary>
        int SongsPerPage { get; }

        /// <summary>
        /// Max number of songs to retrieve, 0 for unlimited.
        /// </summary>
        int MaxSongs { get; }

        /// <summary>
        /// Indicates to the FeedReader that it should store the raw scraped data.
        /// </summary>
        bool StoreRawData { get; }

        /// <summary>
        /// Only return songs that return true for this function.
        /// </summary>
        Func<ScrapedSong, bool>? Filter { get; }

        /// <summary>
        /// If this returns true for any <see cref="ScrapedSong"/>, treat that page as the last.
        /// </summary>
        Func<ScrapedSong, bool>? StopWhenAny { get; }
    }
}
