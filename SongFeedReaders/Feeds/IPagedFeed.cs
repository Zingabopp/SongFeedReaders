using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
    public interface IPagedFeed : IFeed
    {
        /// <summary>
        /// Gets the feed's full URI for the specified page.
        /// </summary>
        /// <param name="page"></param>
        /// <exception cref="InvalidFeedSettingsException">Thrown when the feed's settings aren't valid.</exception>
        /// <returns></returns>
        Uri GetUriForPage(int page);
    }
}
