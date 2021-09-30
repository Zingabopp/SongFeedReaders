using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// This feed returns songs uploaded by a specific Beat Saver user.
    /// </summary>
    public class BeatSaverMapperFeed : BeatSaverFeed, IPagedFeed
    {
        private BeatSaverMapperSettings MapperFeedSettings => (BeatSaverMapperSettings)FeedSettings;
        private readonly object _initializeLock = new object();
        /// <summary>
        /// A dictionary with Beat Saver username keys mapped to author IDs.
        /// </summary>
        protected static readonly ConcurrentDictionary<string, string> AuthorIdMap = new ConcurrentDictionary<string, string>();

        private string? _mapperId;
        /// <summary>
        /// Beat Saver ID for the mapper.
        /// </summary>
        public string? MapperId
        {
            get { return _mapperId; }
            private set { _mapperId = value; }
        }

        /// <summary>
        /// Creates a new <see cref="BeatSaverMapperFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeatSaverMapperFeed(BeatSaverMapperSettings feedSettings, IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <summary>
        /// Creates a new <see cref="BeatSaverMapperFeed"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        public BeatSaverMapperFeed(ISettingsFactory settingsFactory, IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
            : base(settingsFactory, pageHandler, webClient, logFactory)
        {
        }

        /// <inheritdoc/>
        public override string FeedId => "BeatSaver.Mapper";

        /// <inheritdoc/>
        public override string DisplayName => $"Beat Saver Mapper: {MapperFeedSettings.MapperName}";

        /// <inheritdoc/>
        public override string Description => "Songs uploaded by a specific mapper";
        /// <inheritdoc/>
        public override bool Initialized
        {
            get
            {
                return !string.IsNullOrWhiteSpace(MapperId);
            }
        }
        private Task? _initializeTask;
        /// <inheritdoc/>
        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            EnsureValidSettings();
            lock (_initializeLock)
            {
                if(_initializeTask == null || (_initializeTask.IsCompleted && !Initialized))
                {
                    _initializeTask = GetAuthorIDAsync(MapperFeedSettings.MapperName, cancellationToken);
                    return _initializeTask;
                }
            }
            return InitializeWaitAsync(cancellationToken);
        }

        /// <summary>
        /// Waits for an existing Initialization task allowing for a separate cancellation token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task InitializeWaitAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            using var reg = cancellationToken.Register(() => tcs.TrySetCanceled());
            await Task.WhenAny(_initializeTask, tcs.Task);
            tcs.TrySetResult(true);
        }


        /// <inheritdoc/>
        public Uri GetUriForPage(int page)
        {
            EnsureValidSettings();
            if (!Initialized)
                throw new FeedUninitializedException("The feed must be initialized before this operation can be performed.");
            return new Uri(BeatSaverHelper.BeatSaverApiUri, $"maps/uploader/{MapperId}/{page}");
        }

        /// <inheritdoc/>
        protected override bool AreSettingsValid(IFeedSettings settings)
        {
            if (settings is BeatSaverMapperSettings mSettings)
                return !string.IsNullOrWhiteSpace(mSettings.MapperName);
            return false;
        }

        /// <inheritdoc/>
        public override void EnsureValidSettings()
        {
            if (FeedSettings == null)
                throw new InvalidFeedSettingsException("FeedSettings is null.");
            if (!(FeedSettings is BeatSaverMapperSettings settings))
            {
                throw new InvalidFeedSettingsException($"Settings is the wrong type ({FeedSettings?.GetType().Name}), "
                    + $"should be {nameof(BeatSaverMapperSettings)}");
            }
            if (string.IsNullOrWhiteSpace(settings.MapperName))
                throw new InvalidFeedSettingsException($"{nameof(BeatSaverMapperSettings)} must have a {nameof(settings.MapperName)}");
        }

        /// <inheritdoc/>
        public override FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)
        {
            if (!AreSettingsValid(settings))
                throw new InvalidFeedSettingsException();
            if(!Initialized)
                throw new FeedUninitializedException("The feed must be initialized before this operation can be performed.");
            return new PagedFeedAsyncEnumerator(this, 1, 0, Logger);
        }


        /// <summary>
        /// Attempts to retrieve a Beat Saver user ID using the username.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="PageParseException"></exception>
        /// <exception cref="WebClientException"></exception>
        /// <returns></returns>
        public async Task<string> GetAuthorIDAsync(string userName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;
            if (AuthorIdMap.TryGetValue(userName, out string userId))
            {
                Logger?.Debug($"Author '{userName}' in cache with ID '{userId}'");
                return userId;
            }
            IBeatSaverPageHandler pageHandler = PageHandler as IBeatSaverPageHandler
                ?? throw new InvalidOperationException("Unable to look up user ID: This feed's IPageHandler must be an IBeatSaverPageHandler");
            
            Logger?.Debug($"Attempting ID lookup for the user '{userName}'.");
            Uri? sourceUri = new Uri(BeatSaverHelper.BeatSaverApiUri, $"users/name/{userName}");
            IWebResponseMessage? response = null;
            try
            {
                response = await WebClient.GetAsync(sourceUri, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response.Content == null)
                {
                    Logger?.Error($"WebResponse Content was null getting user ID from username '{userName}'");
                    return string.Empty;
                }
                string pageText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string mapperId = pageHandler.ParseUserIdFromPage(pageText);
                if (string.IsNullOrWhiteSpace(mapperId))
                {
                    throw new PageParseException($"Beat Saver ID for username '{userName}' was not found.");
                }

                AuthorIdMap[userName] = mapperId;
                MapperId = mapperId;
                return mapperId;

            }
            catch(PageParseException)
            {
                throw;
            }
            catch (WebClientException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PageParseException($"Uncaught error getting UploaderID from author name '{userName}'", ex);
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}
