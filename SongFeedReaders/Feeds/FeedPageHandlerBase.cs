using SongFeedReaders.Feeds;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Base class for an <see cref="IFeedPageHandler"/>.
    /// </summary>
    public abstract class FeedPageHandlerBase : IFeedPageHandler
    {
        /// <summary>
        /// Logger used by this instance.
        /// </summary>
        protected readonly ILogger? Logger;
        /// <summary>
        /// Creates a new <see cref="FeedPageHandlerBase"/>
        /// </summary>
        protected FeedPageHandlerBase()
        { }
        /// <summary>
        /// Creates a new <see cref="FeedPageHandlerBase"/>
        /// </summary>
        protected FeedPageHandlerBase(ILogger? logger)
        {
            Logger = logger;
        }
        /// <summary>
        /// Creates a new <see cref="FeedPageHandlerBase"/>
        /// </summary>
        protected FeedPageHandlerBase(ILogFactory? logFactory)
        {
            Logger = logFactory?.GetLogger(GetType().Name);
        }

        /// <inheritdoc/>
        public abstract PageReadResult Parse(PageContent content, Uri? pageUri, IFeedSettings settings);

        /// <summary>
        /// Creates a basic <see cref="PageReadResult"/> from the given songs, pageUri, and settings.
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="pageUri"></param>
        /// <param name="settings"></param>
        /// <param name="isLastPage"></param>
        /// <returns></returns>
        protected virtual PageReadResult CreateResult(List<ScrapedSong> songs, Uri? pageUri, IFeedSettings? settings, bool isLastPage = false)
        {
            try
            {
                Func<ScrapedSong, bool>? stopWhenAny = settings?.StopWhenAny;
                ScrapedSong? firstSong = songs.FirstOrDefault();
                ScrapedSong? lastSong = songs.LastOrDefault();
                int songsOnPage = songs.Count;
                List<ScrapedSong> filteredSongs = new List<ScrapedSong>(songsOnPage);
                try
                {
                    foreach (var song in songs)
                    {
                        if (stopWhenAny != null && stopWhenAny(song))
                        {
                            isLastPage = true;
                            break;
                        }
                        if (AcceptSong(song, settings))
                        {
                            filteredSongs.Add(song);
                        }
                    }
                }
                catch (FeedFilterException ex)
                {
                    return new PageReadResult(pageUri, ex, PageErrorType.FilterError,
                        filteredSongs, firstSong, lastSong, songsOnPage, true);
                }
                isLastPage = isLastPage || songs.Count == 0;
                return new PageReadResult(pageUri, filteredSongs,
                    firstSong, lastSong, songsOnPage, isLastPage);
            }
            catch (PageParseException ex)
            {
                return new PageReadResult(pageUri, ex, PageErrorType.ParsingError);
            }
            catch (Exception ex)
            {
                return new PageReadResult(pageUri, ex, PageErrorType.Unknown);
            }
        }

        /// <summary>
        /// Filters songs according to the generic filterable settings in <see cref="IFeedSettings"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="FeedFilterException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual bool AcceptSong(ScrapedSong song, IFeedSettings? settings)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song));
            if (settings == null)
                return true;
            Func<ScrapedSong, bool>? filter = settings.Filter;
            try
            {
                if (filter == null || filter(song))
                    return true;
            }
            catch (Exception ex)
            {
                throw new FeedFilterException($"Error filtering songs from settings '{settings.GetType().Name}': {ex.Message}", ex);
            }
            return false;
        }
    }
}
