using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Handles reading content from a feed page.
    /// <see cref="IFeed"/> implementations should use an interface or base class specific
    /// to their service in their constructors.
    /// </summary>
    public interface IFeedPageHandler
    {
        /// <summary>
        /// Parse's the page's content and returns a list of every <see cref="ScrapedSong"/> on the page.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="pageUri"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PageParseException"></exception>
        List<ScrapedSong> Parse(string content, Uri? pageUri, IFeedSettings settings);
    }
}
