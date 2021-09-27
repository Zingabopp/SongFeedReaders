using SongFeedReaders.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Used to move forward or backward through a feed.
    /// </summary>
    public abstract class FeedAsyncEnumerator
    {
        /// <summary>
        /// Feed to enumerate through.
        /// </summary>
        protected readonly IFeed Feed;
        /// <summary>
        /// Logger used by this object.
        /// </summary>
        protected readonly ILogger? Logger;
        /// <summary>
        /// How many pages to load forward/backward. Page caching must be enabled.
        /// </summary>
        public int LookAhead { get; set; }
        /// <summary>
        /// If true, page results will be stored to speed up access to previously visited pages.
        /// </summary>
        public bool EnablePageCache { get; }
        /// <summary>
        /// If true, indicates there may be another page before the current one.
        /// </summary>
        public virtual bool CanMovePrevious { get; protected set; }
        /// <summary>
        /// If true, indicates there may be another page after the current one.
        /// </summary>
        public virtual bool CanMoveNext { get; protected set; }
        /// <summary>
        /// Creates a new <see cref="FeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected FeedAsyncEnumerator(IFeed feed, ILogger? logger = null)
        {
            Feed = feed ?? throw new ArgumentNullException(nameof(feed));
            Logger = logger;
            CanMoveNext = true;
        }
        /// <summary>
        /// Creates a new <see cref="FeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="cachePages"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected FeedAsyncEnumerator(IFeed feed, bool cachePages, ILogger? logger = null)
            : this(feed, logger)
        {
            EnablePageCache = cachePages;
        }

        /// <summary>
        /// Requests the next page and returns the result.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="FeedReaderException"></exception>
        public abstract Task<PageReadResult> MoveNextAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Requests the previous page and returns the result.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="FeedReaderException"></exception>
        public abstract Task<PageReadResult> MovePreviousAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Requests the next page and returns the result.
        /// </summary>
        /// <returns></returns>
        public Task<PageReadResult> MoveNextAsync()
            => MoveNextAsync(CancellationToken.None);


        /// <summary>
        /// Requests the previous page and returns the result.
        /// </summary>
        /// <returns></returns>
        public Task<PageReadResult> MovePreviousAsync()
            => MovePreviousAsync(CancellationToken.None);
    }
}
