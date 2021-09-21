using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Base class for an <see cref="IFeed"/>.
    /// </summary>
    public abstract class FeedBase : IFeed
    {
        /// <summary>
        /// <see cref="ILogger"/> used by this instance.
        /// </summary>
        protected readonly ILogger? Logger;
        /// <summary>
        /// Parses the contents of Beat Saver pages.
        /// </summary>
        protected IFeedPageHandler PageHandler;

        /// <inheritdoc/>
        public event EventHandler? FeedStarting;
        /// <inheritdoc/>
        public event EventHandler<FeedResult>? FeedFinished;

        /// <inheritdoc/>
        public abstract string FeedId { get; }
        /// <inheritdoc/>
        public abstract string DisplayName { get; }
        /// <inheritdoc/>
        public abstract string Description { get; }
        /// <inheritdoc/>
        public IFeedSettings FeedSettings { get; protected set; }
        /// <inheritdoc/>
        public abstract bool HasValidSettings { get; }

        /// <inheritdoc/>
        public bool ReadInProgress => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new <see cref="FeedBase"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="logFactory"></param>
        protected FeedBase(IFeedSettings feedSettings, IFeedPageHandler pageHandler,
            ILogFactory? logFactory)
        {
            FeedSettings = feedSettings ?? throw new ArgumentNullException(nameof(feedSettings));
            PageHandler = pageHandler ?? throw new ArgumentNullException(nameof(pageHandler));
            Logger = logFactory?.GetLogger();
        }

        /// <inheritdoc/>
        public virtual Task<FeedResult> ReadAsync()
            => ReadAsync(null);

        /// <inheritdoc/>
        public virtual Task<FeedResult> ReadAsync(IProgress<PageReadResult>? progress)
        {

        }
    }
}
