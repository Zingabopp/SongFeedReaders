using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Stores the results of a feed.
    /// </summary>
    public class FeedResult
    {
        /// <summary>
        /// Dictionary of songs that were read from the feed.
        /// </summary>
        protected readonly IDictionary<string, ScrapedSong> songs;
        /// <summary>
        /// Array of page results.
        /// </summary>
        protected readonly PageReadResult[] pageResults;
        /// <summary>
        /// Level of error for the feed result.
        /// </summary>
        public FeedResultErrorLevel ErrorLevel { get; private set; }
        /// <summary>
        /// Exception when something goes wrong in the feed readers. More specific exceptions may be stored in InnerException.
        /// </summary>
        public FeedReaderException? Exception { get; private set; }

        private readonly bool _successful;
        /// <summary>
        /// True if the feed reading was successful.
        /// </summary>
        public bool Successful
        {
            get
            {
                return _successful
&& ErrorLevel != FeedResultErrorLevel.Error
&& ErrorLevel != FeedResultErrorLevel.Cancelled;
            }
        }
        /// <summary>
        /// Number of pages checked.
        /// </summary>
        public int PagesChecked { get { return pageResults?.Length ?? 0; } }
        /// <summary>
        /// Returns the songs from the result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScrapedSong> GetSongs()
            => songs.Values.AsEnumerable();
        /// <summary>
        /// Returns the page results.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageReadResult> GetResults()
            => pageResults.AsEnumerable();

        /// <summary>
        /// Number of songs in the result.
        /// </summary>
        public int Count => songs.Count;
        /// <summary>
        /// Number of songs observed by the feed (including ones that were filtered out).
        /// </summary>
        public int SongsCheckedCount { get; private set; }
        /// <summary>
        /// Creates a new <see cref="FeedResult"/>.
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="pageReadResults"></param>
        public FeedResult(IDictionary<string, ScrapedSong> songs,
            IEnumerable<PageReadResult>? pageReadResults)
        {
            _successful = true;
            this.songs = songs ?? new Dictionary<string, ScrapedSong>();
            if (pageReadResults != null)
            {
                pageResults = pageReadResults.ToArray();
                int pagesWithErrors = 0;
                foreach (PageReadResult? page in pageReadResults)
                {
                    SongsCheckedCount += page.SongsOnPage;
                    if (page.Exception != null)
                    {
                        ErrorLevel = FeedResultErrorLevel.Warning;
                        Exception = page.Exception;
                        pagesWithErrors++;
                    }
                }
                if (pagesWithErrors == pageResults.Length)
                {
                    _successful = false;
                    ErrorLevel = FeedResultErrorLevel.Error;
                }
            }
            else
            {
                pageResults = Array.Empty<PageReadResult>();
                _successful = false;
            }
        }
        /// <summary>
        /// Creates a new <see cref="FeedResult"/> where an error occurred.
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="pageResults"></param>
        /// <param name="exception"></param>
        /// <param name="errorLevel"></param>
        public FeedResult(IDictionary<string, ScrapedSong>? songs,
            IEnumerable<PageReadResult>? pageResults,
            Exception? exception,
            FeedResultErrorLevel errorLevel)
           : this(songs ?? new Dictionary<string, ScrapedSong>(), pageResults)
        {
            _successful = false;
            if (ErrorLevel < errorLevel)
                ErrorLevel = errorLevel;

            if (exception != null)
            {
                if (exception is FeedReaderException frException)
                {
                    Exception = frException;
                }
                else if (exception is OperationCanceledException canceledException)
                {
                    ErrorLevel = FeedResultErrorLevel.Cancelled;
                    Exception = new FeedReaderException(canceledException.Message, canceledException, FeedReaderFailureCode.Cancelled);
                }
                else
                    Exception = new FeedReaderException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Returns a new cancelled <see cref="FeedResult"/> for a partially complete feed.
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="pageResults"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FeedResult GetCancelledResult(Dictionary<string, ScrapedSong>? songs, IList<PageReadResult>? pageResults, OperationCanceledException ex)
        {
            return new FeedResult(songs, pageResults, ex, FeedResultErrorLevel.Cancelled);
        }
        /// <summary>
        /// Returns a new cancelled <see cref="FeedResult"/> for a partially complete feed.
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="pageResults"></param>
        /// <returns></returns>
        public static FeedResult GetCancelledResult(Dictionary<string, ScrapedSong>? songs, IList<PageReadResult>? pageResults)
        {
            return new FeedResult(songs, pageResults, new OperationCanceledException("Feed was cancelled before completion"), FeedResultErrorLevel.Cancelled);
        }
        /// <summary>
        /// Returns a new cancelled <see cref="FeedResult"/>.
        /// </summary>
        public static FeedResult CancelledResult => GetCancelledResult(null, null);
    }


    /// <summary>
    /// Error level of a feed result.
    /// </summary>
    public enum FeedResultErrorLevel
    {
        /// <summary>
        /// No error occurred.
        /// </summary>
        None = 0,
        /// <summary>
        /// Non-critical error(s) occurred.
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Critical error occurred.
        /// </summary>
        Error = 2,
        /// <summary>
        /// Feed was cancelled.
        /// </summary>
        Cancelled = 3
    }
}
