using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebUtilities;

namespace SongFeedReaders.Feeds
{
    public class PageReadResult
    {
        /// <summary>
        /// <see cref="System.Uri"/> for the page.
        /// </summary>
        public Uri Uri { get; private set; }
        private readonly ScrapedSong[] _songs;
        /// <summary>
        /// 
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

        public bool IsLastPage { get; private set; }

        public PageErrorType PageError { get; private set; }
        public int SongCount { get { return _songs.Length; } }
        public FeedReaderException? Exception { get; private set; }

        private bool _successful;
        public bool Successful { get { return _successful && Exception == null; } }
        public PageReadResult(Uri uri, List<ScrapedSong>? songs, ScrapedSong? firstSong, ScrapedSong? lastSong, 
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
                songs = new List<ScrapedSong>();
            }
            else
                _successful = true;
            _songs = songs?.ToArray() ?? Array.Empty<ScrapedSong>();
        }

        public PageReadResult(Uri uri, Exception? exception, PageErrorType pageError,
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

        public override string ToString()
        {
            return $"{Uri} | {_songs.Length.ToString() ?? "<NULL>"}";
        }

        public static PageReadResult FromWebClientException(WebClientException? ex, Uri requestUri, ILogger logger)
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

        public static PageReadResult CancelledResult(Uri requestUri, OperationCanceledException ex)
        {
            return new PageReadResult(requestUri, ex, PageErrorType.Cancelled);
        }

        public static PageReadResult CancelledResult(Uri requestUri)
        {
            return new PageReadResult(requestUri, new OperationCanceledException(), PageErrorType.Cancelled);
        }
    }


    public static class PageErrorTypeExtensions
    {
        public static string ErrorToString(this PageErrorType pageError)
        {
            switch (pageError)
            {
                case PageErrorType.None:
                    return string.Empty;
                case PageErrorType.Timeout:
                    return "Timeout";
                case PageErrorType.SiteError:
                    return "Site Error";
                case PageErrorType.ParsingError:
                    return "Parsing Error";
                case PageErrorType.PageOutOfRange:
                    return "Page out of range";
                case PageErrorType.Unknown:
                    return "Unknown Error";
                default:
                    return "Unknown Error";
            }
        }
    }
    public enum PageErrorType
    {
        None = 0,
        Timeout = 1,
        SiteError = 2,
        ParsingError = 3,
        Cancelled = 4,
        Unknown = 5,
        PageOutOfRange = 6
    }
}
