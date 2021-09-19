using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Settings for an <see cref="IDatedFeed"/>.
    /// </summary>
    public interface IDatedFeedSettings : IFeedSettings
    {
        /// <summary>
        /// Start reading song
        /// </summary>
        DateTime StartingDate { get; set; }
        DateTime EndingDate { get; set; }
    }
}
