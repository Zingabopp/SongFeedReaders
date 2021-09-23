using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Iterates through feed pages by their page number.
    /// </summary>
    public class PagedFeedAsyncEnumerator : FeedAsyncEnumerator
    {
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// <see cref="IPagedFeed"/> used by this instance.
        /// </summary>
        public readonly IPagedFeed PagedFeed;
        /// <summary>
        /// The first page of the feed.
        /// </summary>
        protected readonly int FeedFirstPage;
        private int currentPage;
        private Uri? LastFetchedUri;

        /// <summary>
        /// Creates a new <see cref="PagedFeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="feedFirstPage"></param>
        /// <param name="startingPage"></param>
        public PagedFeedAsyncEnumerator(IPagedFeed feed, int startingPage, int feedFirstPage)
            : this(feed, startingPage, feedFirstPage, false)
        {
        }
        /// <summary>
        /// Creates a new <see cref="PagedFeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="startingPage"></param>
        /// <param name="feedFirstPage"></param>
        /// <param name="cachePages"></param>
        public PagedFeedAsyncEnumerator(IPagedFeed feed, int startingPage, int feedFirstPage, bool cachePages)
            : base(feed, cachePages)
        {
            if (startingPage < feedFirstPage)
                throw new ArgumentException($"startingPage '{startingPage}' can't be less than the feed's first page '{feedFirstPage}'");
            currentPage = startingPage - 1; // First MoveNext increments
            PagedFeed = feed;
            FeedFirstPage = feedFirstPage;
        }

        /// <inheritdoc/>
        public override async Task<PageReadResult> MoveNextAsync(CancellationToken cancellationToken)
        {

            PageReadResult? result = null;
            Uri? pageUri = null;
            bool criticalFailure = false;
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                // TODO: CanMoveNext/CanMovePrevious can give false values if multiple threads are using the same object
                int page = Interlocked.Increment(ref currentPage);
                CanMovePrevious = currentPage > FeedFirstPage;
                pageUri = PagedFeed.GetUriForPage(page);
                if (pageUri == LastFetchedUri)
                {
                    criticalFailure = true;
                    throw new FeedReaderException($"URL '{pageUri}' was previously read, aborting to avoid possible infinite loop.");
                }
                result = await PagedFeed.GetPageAsync(pageUri, cancellationToken).ConfigureAwait(false);
                //if (result.IsLastPage)
                //    CanMoveNext = false;
                LastFetchedUri = pageUri;
            }
            catch (OperationCanceledException ex)
            {
                result = PageReadResult.CancelledResult(pageUri, ex);
            }
            catch (Exception ex)
            {
                if (criticalFailure) // Break everything
                    throw;

                result = new PageReadResult(
                    uri: pageUri,
                    exception: ex,
                    pageError: PageErrorType.Unknown);
            }
            finally
            {

                _semaphore.Release();
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<PageReadResult> MovePreviousAsync(CancellationToken cancellationToken)
        {
            PageReadResult? result = null;
            Uri? pageUri = null;
            bool criticalFailure = false;
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                // CanMovePrevious can give false values if multiple threads are using the same object
                int page = Interlocked.Decrement(ref currentPage);
                CanMovePrevious = currentPage > FeedFirstPage;
                pageUri = PagedFeed.GetUriForPage(page);
                if (pageUri == LastFetchedUri)
                {
                    criticalFailure = true;
                    throw new FeedReaderException($"URL '{pageUri}' was previously read, aborting to avoid possible infinite loop.");
                }
                result = await PagedFeed.GetPageAsync(pageUri, cancellationToken).ConfigureAwait(false);
                LastFetchedUri = pageUri;
            }
            catch (OperationCanceledException ex)
            {
                result = PageReadResult.CancelledResult(pageUri, ex);
            }
            catch (Exception ex)
            {
                if (criticalFailure) // Break everything
                    throw;

                result = new PageReadResult(
                    uri: pageUri,
                    exception: ex,
                    pageError: PageErrorType.Unknown);
            }
            finally
            {

                _semaphore.Release();
            }

            return result;
        }
    }
}
