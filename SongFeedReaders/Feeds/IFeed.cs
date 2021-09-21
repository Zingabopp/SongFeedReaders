using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Represents a specific feed for a service.
    /// </summary>
    public interface IFeed
    {
        /// <summary>
        /// Unique string ID of the feed. (Format should be 'Service.FeedName').
        /// </summary>
        string FeedId { get; }
        /// <summary>
        /// Display name of the feed.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Description of the feed.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Settings for the feed.
        /// </summary>
        IFeedSettings FeedSettings { get; }
        /// <summary>
        /// Return true if the settings are valid for this feed.
        /// </summary>
        bool HasValidSettings { get; }
        /// <summary>
        /// Returns true if the feed is currently running.
        /// </summary>
        bool ReadInProgress { get; }

        /// <summary>
        /// Raised when the feed has started reading.
        /// </summary>
        event EventHandler? FeedStarting;
        /// <summary>
        /// Raised when the feed has finished reading.
        /// </summary>
        event EventHandler<FeedResult>? FeedFinished;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<FeedResult> ReadAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<FeedResult> ReadAsync(IProgress<PageReadResult> progress);
    }
}
