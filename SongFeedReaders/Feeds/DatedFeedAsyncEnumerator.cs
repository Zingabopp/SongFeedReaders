using SongFeedReaders.Logging;
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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
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
        /// <param name="logger"></param>
        public DatedFeedAsyncEnumerator(IDatedFeed datedFeed, bool cachePages = false,
            ILogger? logger = null)
            : this(datedFeed, FeedDate.Default, cachePages, logger) { }

        /// <summary>
        /// Creates a new <see cref="FeedAsyncEnumerator"/>.
        /// </summary>
        /// <param name="datedFeed"></param>
        /// <param name="feedDate"></param>
        /// <param name="cachePages"></param>
        /// <param name="logger"></param>
        public DatedFeedAsyncEnumerator(IDatedFeed datedFeed, FeedDate feedDate,
            bool cachePages = false, ILogger? logger = null)
            : base(datedFeed, cachePages, logger)
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
                //Logger?.Debug($"Latest: {result.FirstSong?.Key}: {CurrentLatest.ToLocalTime()}");
                //Logger?.Debug($"Earliest: {result.LastSong?.Key}: {CurrentEarliest.ToLocalTime()}");
                if (dateDirection == DateDirection.Before)
                {
                    CanMoveNext = true;
                    CanMovePrevious = CurrentLatest < Utilities.Util.Now;
                }
                else
                {
                    CanMoveNext = true;
                    CanMovePrevious = CurrentLatest < Utilities.Util.Now; ;
                }
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
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                FeedDate feedDate = new FeedDate(CurrentEarliest, DateDirection.Before);
                result = await MoveAsync(feedDate, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                result = PageReadResult.CancelledResult(null, ex);
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
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                FeedDate feedDate = new FeedDate(CurrentLatest, DateDirection.After);
                result = await MoveAsync(feedDate, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                result = PageReadResult.CancelledResult(null, ex);
            }
            finally
            {
                _semaphore.Release();
            }
            return result;
        }

        private async Task<PageReadResult> MoveAsync(FeedDate feedDate, CancellationToken cancellationToken)
        {
            PageReadResult? result = null;
            Uri? pageUri = null;
            bool criticalFailure = false;
            try
            {
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
            }

            return result;
        }
    }


}
