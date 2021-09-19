using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
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
