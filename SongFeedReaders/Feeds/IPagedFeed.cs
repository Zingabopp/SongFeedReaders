using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// A feed that lists songs by page.
    /// </summary>
    public interface IPagedFeed : IFeed
    {
        /// <summary>
        /// Gets the feed's full URI for the specified page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException">Thrown when the feed's settings aren't valid.</exception>
        /// <exception cref="FeedUninitializedException">Thrown when the feed hasn't been initialized.</exception>
        Uri GetUriForPage(int page);
    }
}
