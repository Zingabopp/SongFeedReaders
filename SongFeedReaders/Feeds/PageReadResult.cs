using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebUtilities;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Stores the results of a page read.
    /// </summary>
    public class PageReadResult
    {
        private readonly ScrapedSong[] _songs;
        /// <summary>
        /// <see cref="System.Uri"/> for the page.
        /// </summary>
        public Uri? Uri { get; private set; }
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of the matched songs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScrapedSong> Songs() => _songs.AsEnumerable();
        /// <summary>
        /// First song on the unfiltered page.
        /// </summary>
        public readonly ScrapedSong? FirstSong;
        /// <summary>
        /// Last song on the unfiltered page.
        /// </summary>
        public readonly ScrapedSong? LastSong;
        /// <summary>
        /// Total unfiltered songs.
        /// </summary>
        public readonly int SongsOnPage;
        /// <summary>
        /// If true, indicates there are no more pages after this one.
        /// </summary>
        public bool IsLastPage { get; private set; }
        /// <summary>
        /// Type of error that occurred, if any.
        /// </summary>
        public PageErrorType PageError { get; private set; }
        /// <summary>
        /// Number of songs returned by the page.
        /// </summary>
        public int SongCount { get { return _songs.Length; } }
        /// <summary>
        /// If an exception was thrown reading the page, it is available here.
        /// </summary>
        public FeedReaderException? Exception { get; private set; }

        private readonly bool _successful;
        /// <summary>
        /// If true, indicates the reading of the page was successful.
        /// </summary>
        public bool Successful { get { return _successful && Exception == null; } }
        /// <summary>
        /// Create a new <see cref="PageReadResult"/>.
        /// </summary>
        /// <param name="uri"><see cref="System.Uri"/> for the page.</param>
        /// <param name="songs">The collection of matched songs. Songs unwanted due to the settings should not be in here.</param>
        /// <param name="firstSong">The first unfiltered song on the page.</param>
        /// <param name="lastSong">The last unfiltered song on the page.</param>
        /// <param name="songsOnPage">Number of unfiltered songs on the page.</param>
        /// <param name="isLastPage">If true, ndicates there are no more pages after this one.</param>
        public PageReadResult(Uri? uri, IEnumerable<ScrapedSong>? songs, ScrapedSong? firstSong, ScrapedSong? lastSong, 
            int songsOnPage, bool isLastPage = false)
        {
            IsLastPage = isLastPage;
            FirstSong = firstSong;
            LastSong = lastSong;
            SongsOnPage = songsOnPage;
            Uri = uri;
            if (songs == null)
            {
                _successful = false;
                _songs = Array.Empty<ScrapedSong>();
            }
            else
            {
                _successful = true;
                _songs = songs.ToArray();

            }
        }
        /// <summary>
        /// Creates a new <see cref="PageReadResult"/> when there were error(s) reading the page.
        /// </summary>
        /// <param name="uri"><see cref="System.Uri"/> for the page.</param>
        /// <param name="exception"></param>
        /// <param name="pageError"></param>
        /// <param name="songs">The collection of matched songs. Songs unwanted due to the settings should not be in here.</param>
        /// <param name="firstSong">The first unfiltered song on the page.</param>
        /// <param name="lastSong">The last unfiltered song on the page.</param>
        /// <param name="songsOnPage">Number of unfiltered songs on the page.</param>
        /// <param name="isLastPage">If true, ndicates there are no more pages after this one.</param>
        public PageReadResult(Uri? uri, Exception? exception, PageErrorType pageError,
            List<ScrapedSong>? songs = null, ScrapedSong? firstSong = null, ScrapedSong? lastSong = null, 
            int songsOnPage = 0, bool isLastPage = false)
            : this(uri, songs, firstSong, lastSong, songsOnPage, isLastPage)
        {
            if (exception != null)
            {
                if (pageError == PageErrorType.None)
                    pageError = PageErrorType.Unknown;
                PageError = pageError;
                if (exception is FeedReaderException frException)
                {
                    Exception = frException;
                }
                else
                    Exception = new FeedReaderException(exception.Message, exception);
            }
            else
            {
                if (pageError > PageError)
                    PageError = pageError;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{_songs.Length.ToString() ?? "<NULL>"} | {Uri}";
        }

        /// <summary>
        /// Creates an empty <see cref="PageReadResult"/> from a <see cref="WebClientException"/>.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="requestUri"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static PageReadResult FromWebClientException(WebClientException? ex, Uri requestUri, ILogger? logger)
        {
            PageErrorType pageError = PageErrorType.SiteError;
            string errorText = string.Empty;
            int statusCode = ex?.Response?.StatusCode ?? 0;
            if (statusCode != 0)
            {
                switch (statusCode)
                {
                    case 408:
                        errorText = "Timeout";
                        pageError = PageErrorType.Timeout;
                        break;
                    default:
                        errorText = "Site Error";
                        pageError = PageErrorType.SiteError;
                        break;
                }
            }
            string message = $"{errorText} getting page '{requestUri}'.";
            logger?.Debug(message);
            // No need for a stacktrace if it's one of these errors.
            if (!(pageError == PageErrorType.Timeout || statusCode == 500) && ex != null)
                logger?.Debug($"{ex.Message}\n{ex.StackTrace}");
            return new PageReadResult(requestUri, new FeedReaderException(message, ex, FeedReaderFailureCode.PageFailed),
                 pageError);
        }

        /// <summary>
        /// Returns an empty <see cref="PageReadResult"/> indicating the page read was cancelled.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static PageReadResult CancelledResult(Uri? requestUri, OperationCanceledException ex)
        {
            return new PageReadResult(requestUri, ex, PageErrorType.Cancelled);
        }

        /// <summary>
        /// Returns an empty <see cref="PageReadResult"/> indicating the page read was cancelled.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static PageReadResult CancelledResult(Uri? requestUri)
        {
            return new PageReadResult(requestUri, new OperationCanceledException(), PageErrorType.Cancelled);
        }
    }

    /// <summary>
    /// Extension method(s) for <see cref="PageErrorType"/>.
    /// </summary>
    public static class PageErrorTypeExtensions
    {
        /// <summary>
        /// Returns a user-friendly string for a given <see cref="PageErrorType"/>.
        /// </summary>
        /// <param name="pageError"></param>
        /// <returns></returns>
        public static string ErrorToString(this PageErrorType pageError)
        {
            return pageError switch
            {
                PageErrorType.None => string.Empty,
                PageErrorType.Timeout => "Timeout",
                PageErrorType.SiteError => "Site Error",
                PageErrorType.ParsingError => "Parsing Error",
                PageErrorType.PageOutOfRange => "Page out of range",
                PageErrorType.Unknown => "Unknown Error",
                PageErrorType.Cancelled => "Operation cancelled",
                PageErrorType.FilterError => "Error in filter",
                _ => "Unknown Error",
            };
        }
    }
    /// <summary>
    /// The type of error, if any, that occurred.
    /// </summary>
    public enum PageErrorType
    {
        /// <summary>
        /// No error occured.
        /// </summary>
        None = 0,
        /// <summary>
        /// A timeout occurred waiting for a response.
        /// </summary>
        Timeout = 1,
        /// <summary>
        /// An error occurred getting a response from the site.
        /// </summary>
        SiteError = 2,
        /// <summary>
        /// Page content was read, but couldn't be parsed.
        /// </summary>
        ParsingError = 3,
        /// <summary>
        /// Page read was cancelled.
        /// </summary>
        Cancelled = 4,
        /// <summary>
        /// An unknown error occurred.
        /// </summary>
        Unknown = 5,
        /// <summary>
        /// A page was requested that is out of range.
        /// </summary>
        PageOutOfRange = 6,
        /// <summary>
        /// An error occurred while filtering songs.
        /// </summary>
        FilterError = 7
    }
}
