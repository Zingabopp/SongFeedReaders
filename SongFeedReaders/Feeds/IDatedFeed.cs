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
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException">Thrown when the feed's settings aren't valid.</exception>
        /// <exception cref="FeedUninitializedException">Thrown when the feed hasn't been initialized.</exception>
        Uri GetUriForDate(FeedDate feedDate);
    }
}
