﻿using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        /// Throws an <see cref="InvalidFeedSettingsException"/> if the settings aren't valid for this feed.
        /// </summary>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        void EnsureValidSettings();

        /// <summary>
        /// Reads and parses a page at the given <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PageReadResult> GetPageAsync(Uri uri, CancellationToken cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        Task<FeedResult> ReadAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        Task<FeedResult> ReadAsync(IProgress<PageReadResult> progress, CancellationToken cancellationToken);
    }
}
