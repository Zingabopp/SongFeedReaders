using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// A <see cref="FeedAsyncEnumerator"/> for feeds that page by DateTime.
    /// </summary>
    public class DatedFeedAsyncEnumerator : FeedAsyncEnumerator
    {
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// The feed associated with this object.
        /// </summary>
        public IDatedFeed DatedFeed { get; }
        /// <summary>
        /// Current page's earliest song upload DateTime.
        /// </summary>
        public DateTime CurrentEarliest { get; protected set; }
        /// <summary>
        /// Current page's latest song upload DateTime.
        /// </summary>
        public DateTime CurrentLatest { get; protected set; }

        private Uri? LastFetchedUri;
        /// <summary>
        /// Creates a new <see cref="FeedAsyncEnumerator"/> using a default <see cref="FeedDate"/>
        /// (starts at the latest song and moves backward).
        /// </summary>
        /// <param name="datedFeed"></param>
        /// <param name="cachePages"></param>
        public DatedFeedAsyncEnumerator(IDatedFeed datedFeed, bool cachePages = false)
            : this(datedFeed, FeedDate.Default, cachePages) { }

        /// <summary>
        /// Creates a new <see cref="FeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="datedFeed"></param>
        /// <param name="feedDate"></param>
        /// <param name="cachePages"></param>
        public DatedFeedAsyncEnumerator(IDatedFeed datedFeed, FeedDate feedDate, bool cachePages = false)
            : base(datedFeed, cachePages)
        {
            CurrentEarliest = feedDate.Date;
            CurrentLatest = feedDate.Date;
            DatedFeed = datedFeed;
        }

        /// <summary>
        /// Process the result to update enumerator's state.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="dateDirection"></param>
        private void ProcessResult(PageReadResult? result, DateDirection dateDirection)
        {
            if (result == null || !result.Successful)
            {
                CanMoveNext = false;
                CanMovePrevious = false;
            }
            else if (result.SongsOnPage > 0)
            {
                // First/Last song should never be null if SongsOnPage > 0.
                CurrentLatest = result.FirstSong!.UploadDate;
                CurrentEarliest = result.LastSong!.UploadDate;
                if (dateDirection == DateDirection.Before)
                    CanMoveNext = true;
                else
                    CanMovePrevious = true;
            }
            else
            {
                if (dateDirection == DateDirection.Before)
                    CanMoveNext = false;
                else
                    CanMovePrevious = false;
            }
        }
        /// <inheritdoc/>
        public override async Task<PageReadResult> MoveNextAsync(CancellationToken cancellationToken)
        {
            PageReadResult? result = null;
            Uri? pageUri = null;
            FeedDate feedDate = default;
            bool criticalFailure = false;
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                feedDate = new FeedDate(CurrentEarliest, DateDirection.Before);
                pageUri = DatedFeed.GetUriForDate(feedDate);
                if (pageUri == LastFetchedUri)
                {
                    criticalFailure = true;
                    throw new FeedReaderException($"URL '{pageUri}' was previously read, aborting to avoid possible infinite loop.");
                }
                result = await DatedFeed.GetPageAsync(pageUri, cancellationToken).ConfigureAwait(false);
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
                ProcessResult(result, feedDate.Direction);
                _semaphore.Release();
            }

            return result;
        }

        /// <inheritdoc/>
        public override Task<PageReadResult> MovePreviousAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }


}
