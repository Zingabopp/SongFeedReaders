using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// A feed that pages songs based on their upload date.
    /// </summary>
    public interface IDatedFeed
    {
        /// <summary>
        /// Gets the feed's full URI for the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <exception cref="InvalidFeedSettingsException">Thrown when the feed's settings aren't valid.</exception>
        /// <returns></returns>
        Uri GetUriForDate(DateTime dateTime);
    }
}
