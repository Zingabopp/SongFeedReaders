using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Interface for a feed that lists songs by page.
    /// </summary>
    public interface IPagedFeed : IFeed
    {
        /// <summary>
        /// Gets the feed settings as an <see cref="IPagedFeedSettings"/>.
        /// </summary>
        /// <returns></returns>
        IPagedFeedSettings? GetPagedFeedSettings();
        /// <summary>
        /// The page number of the first page (usually 0 or 1).
        /// </summary>
        int FeedStartingPage { get; }
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
