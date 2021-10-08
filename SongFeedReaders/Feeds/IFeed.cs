using SongFeedReaders.Utilities;
using System;
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
        /// Unique string iD of the service. (i.e. 'BeatSaver').
        /// </summary>
        string ServiceId { get; }
        /// <summary>
        /// Display name of the feed.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Description of the feed.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Returns true if the feed has been initialized.
        /// </summary>
        bool Initialized { get; }
        /// <summary>
        /// Return true if the settings are valid for this feed.
        /// </summary>
        bool HasValidSettings { get; }
        /// <summary>
        /// Returns the feed settings.
        /// </summary>
        /// <returns></returns>
        IFeedSettings? GetFeedSettings();
        /// <summary>
        /// Performs any work needed to initialize the feed.
        /// Implementing feeds should gracefully handle repeated calls.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        /// <exception cref="FeedInitializationException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task InitializeAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Attempts to assign the given <paramref name="feedSettings"/>. Will return false
        /// if settings have already been assigned.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        bool TryAssignSettings(IFeedSettings feedSettings);
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
        /// Reads the feed according to the current settings and returns a <see cref="FeedResult"/>.
        /// </summary>
        /// <param name="pauseToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        /// <exception cref="FeedUninitializedException"></exception>
        Task<FeedResult> ReadAsync(PauseToken pauseToken, CancellationToken cancellationToken);
        /// <summary>
        /// Reads the feed according to the current settings with progress reports after every page
        /// and returns a <see cref="FeedResult"/>.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="pauseToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        /// <exception cref="FeedUninitializedException"></exception>
        Task<FeedResult> ReadAsync(IProgress<PageReadResult> progress, PauseToken pauseToken, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a <see cref="FeedAsyncEnumerator"/> for this feed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        /// <exception cref="FeedUninitializedException"></exception>
        public FeedAsyncEnumerator GetAsyncEnumerator();
    }

    /// <summary>
    /// Interface for an <see cref="IFeed"/> with a specific <see cref="IFeedSettings"/>.
    /// </summary>
    /// <typeparam name="TFeedSettings"></typeparam>
    public interface IFeed<TFeedSettings> : IFeed
        where TFeedSettings : class, IFeedSettings
    {
        /// <summary>
        /// Settings for the feed. Should only be null if <see cref="IFeed.Initialized"/> is false.
        /// </summary>
        TFeedSettings? FeedSettings { get; }

    }
}
