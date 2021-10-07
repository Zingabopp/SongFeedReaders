using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Settings for an <see cref="IDatedFeed"/>.
    /// </summary>
    public interface IDatedFeedSettings : IFeedSettings
    {
        /// <summary>
        /// Start reading songs uploaded on or after this date.
        /// </summary>
        DateTime StartingDate { get; set; }
        /// <summary>
        /// Stop reading songs uploaded after this date.
        /// </summary>
        DateTime EndingDate { get; set; }
    }
}
