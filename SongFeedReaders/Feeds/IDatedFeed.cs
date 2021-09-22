using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// A feed that pages songs based on their upload date.
    /// </summary>
    public interface IDatedFeed : IFeed
    {
        /// <summary>
        /// Gets the feed's full URI for the specified <see cref="FeedDate"/>.
        /// </summary>
        /// <param name="feedDate"></param>
        /// <exception cref="InvalidFeedSettingsException">Thrown when the feed's settings aren't valid.</exception>
        /// <returns></returns>
        Uri GetUriForDate(FeedDate feedDate);
    }
}
