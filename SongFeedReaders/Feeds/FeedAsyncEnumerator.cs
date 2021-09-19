using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Feeds
{
    public abstract class FeedAsyncEnumerator
    {
        protected readonly IFeed Feed;

        /// <summary>
        /// How many pages to load forward/backward. Page caching must be enabled.
        /// </summary>
        public int LookAhead { get; set; }

        public bool EnablePageCache { get; }

        public virtual bool CanMovePrevious { get; protected set; }
        public virtual bool CanMoveNext { get; protected set; }

        protected FeedAsyncEnumerator(IFeed feed)
        {
            Feed = feed ?? throw new ArgumentNullException(nameof(feed));
            CanMoveNext = true;
        }
        protected FeedAsyncEnumerator(IFeed feed, bool cachePages)
        {
            Feed = feed ?? throw new ArgumentNullException(nameof(feed));
            EnablePageCache = cachePages;
        }

        public abstract Task<PageReadResult> MoveNextAsync(CancellationToken cancellationToken);


        public abstract Task<PageReadResult> MovePreviousAsync(CancellationToken cancellationToken);
    }
}
