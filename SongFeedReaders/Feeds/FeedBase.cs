using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Base class for an <see cref="IFeed"/>.
    /// </summary>
    public abstract class FeedBase<TFeedSettings> : IFeed<TFeedSettings>
        where TFeedSettings : class, IFeedSettings
    {
        private static readonly Task<bool> CompletedInitialization = Task.FromResult(true);
        /// <summary>
        /// <see cref="ILogger"/> used by this instance.
        /// </summary>
        protected readonly ILogger? Logger;
        /// <summary>
        /// Parses the contents of Beat Saver pages.
        /// </summary>
        protected IFeedPageHandler PageHandler;
        /// <summary>
        /// Web client used by the feed.
        /// </summary>
        protected readonly IWebClient WebClient;
        /// <inheritdoc/>
        public abstract string FeedId { get; }
        /// <inheritdoc/>
        public abstract string ServiceId { get; }
        /// <inheritdoc/>
        public abstract string DisplayName { get; }
        /// <inheritdoc/>
        public abstract string Description { get; }
        /// <inheritdoc/>
        public virtual bool Initialized => FeedSettings != null;
        /// <inheritdoc/>
        IFeedSettings? IFeed.GetFeedSettings() => FeedSettings;
        /// <inheritdoc/>
        public TFeedSettings? FeedSettings { get; protected set; }
        /// <inheritdoc/>
        public bool HasValidSettings => FeedSettings != null && AreSettingsValid(FeedSettings);


        /// <inheritdoc/>
        public abstract void EnsureValidSettings();

        /// <summary>
        /// Creates a new <see cref="FeedBase{TFeedSettings}"/>.
        /// </summary>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected FeedBase(IFeedPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
        {
            PageHandler = pageHandler ?? throw new ArgumentNullException(nameof(pageHandler));
            WebClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
            Logger = logFactory?.GetLogger(GetType().Name);
        }

        /// <summary>
        /// Returns a <see cref="PageContent"/> from <paramref name="responseContent"/>.
        /// </summary>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        protected abstract Task<PageContent> GetPageContent(IWebResponseContent responseContent);

        /// <inheritdoc/>
        public virtual Task InitializeAsync(CancellationToken cancellationToken)
        {
            EnsureValidSettings();
            return CompletedInitialization;
        }

        /// <summary>
        /// Attempts to assign the given <paramref name="feedSettings"/>. Will return false
        /// if settings have already been assigned.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        public bool TryAssignSettings(TFeedSettings feedSettings)
        {
            if (feedSettings == null)
                return false;
            FeedSettings ??= feedSettings;
            try
            {
                EnsureValidSettings();
            }
            catch
            {
                FeedSettings = null;
                throw;
            }
            return FeedSettings == feedSettings;
        }
        /// <inheritdoc/>
        bool IFeed.TryAssignSettings(IFeedSettings feedSettings)
        {
            if (feedSettings == null)
                return false;
            if (feedSettings is TFeedSettings settings)
            {
                return TryAssignSettings(settings);
            }
            throw new InvalidFeedSettingsException($"{FeedId} does not accept settings of type '{feedSettings.GetType().Name}'.");
        }

        /// <inheritdoc/>
        public virtual async Task<PageReadResult> GetPageAsync(Uri uri, CancellationToken cancellationToken)
        {
            try
            {
                IWebResponseMessage response = await WebClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                IWebResponseContent? content = response.Content;
                if (content == null)
                    throw new WebClientException("Content was null even though the response succeeded...");
                PageContent pageContent = await GetPageContent(content).ConfigureAwait(false);
                return ParseSongsFromPage(pageContent, uri);
            }
            catch (OperationCanceledException ex)
            {
                return PageReadResult.CancelledResult(uri, ex);
            }
            catch (WebClientException ex)
            {
                return PageReadResult.FromWebClientException(ex, uri, Logger);
            }
            catch (PageParseException ex)
            {
                return new PageReadResult(
                    uri: uri,
                    exception: ex,
                    pageError: PageErrorType.ParsingError);
            }
            catch (Exception ex)
            {
                return new PageReadResult(
                    uri: uri,
                    exception: ex,
                    pageError: PageErrorType.Unknown);

            }
        }

        /// <inheritdoc/>
        public virtual Task<FeedResult> ReadAsync(PauseToken pauseToken, CancellationToken cancellationToken)
            => ReadAsync(null, pauseToken, cancellationToken);

        /// <inheritdoc/>
        public virtual async Task<FeedResult> ReadAsync(IProgress<PageReadResult>? progress, PauseToken pauseToken, CancellationToken cancellationToken)
        {
            IFeedSettings settings = FeedSettings!; // EnsureValidSettings() checks for null.
            EnsureValidSettings();
            try
            {
                if (!Initialized)
                {
                    throw new FeedUninitializedException();
                }
                FeedAsyncEnumerator asyncEnumerator = GetAsyncEnumerator();
                List<PageReadResult> pageResults = new List<PageReadResult>();
                int dictInitialSize = settings.MaxSongs > 0 ? settings.MaxSongs : 20;
                Dictionary<string, ScrapedSong> acceptedSongs = new Dictionary<string, ScrapedSong>(dictInitialSize);
                int maxSongs = settings.MaxSongs > 0 ? settings.MaxSongs : int.MaxValue;
                int songCount = 0;
                int resultsWithZeroSongs = 0;
                PageReadResult? lastResult = null;
                bool breakWhile = false;
                while (!breakWhile && asyncEnumerator.CanMoveNext && !(lastResult?.IsLastPage ?? false))
                {
                    if (pauseToken.CanPause)
                    {
                        bool pauseRequested = pauseToken.IsPauseRequested;
                        Stopwatch? sw = null;
                        if (pauseRequested)
                        {
                            sw = new Stopwatch();
                            sw.Start();
                            Logger?.Debug($"Pause is requested.");
                        }
                        await pauseToken.WaitForPauseAsync(cancellationToken).ConfigureAwait(false);
                        if (sw != null)
                        {
                            sw.Stop();
                            Logger?.Debug($"Resumed after {sw.Elapsed.TotalSeconds} seconds");
                        }
                    }
                    lastResult = await asyncEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
                    songCount += lastResult.SongCount;
                    pageResults.Add(lastResult);

                    progress?.Report(lastResult);
                    if (!lastResult.Successful)
                    {
                        return new FeedResult(acceptedSongs, pageResults, lastResult.Exception, FeedResultErrorLevel.Error);
                    }
                    if (lastResult.SongsOnPage == 0)
                    {
                        resultsWithZeroSongs++;
                    }
                    int acceptedFromPage = 0;
                    foreach (ScrapedSong? song in lastResult.Songs())
                    {
                        string? songHash = song.Hash;
                        if (songHash != null && songHash.Length > 0)
                        {
                            if (!acceptedSongs.ContainsKey(songHash))
                            {
                                acceptedSongs[songHash] = song;
                                acceptedFromPage++;
                            }
                            if (acceptedSongs.Count >= maxSongs)
                            {
                                breakWhile = true;
                                break;
                            }
                        }
                    }
                    if (lastResult.SongsOnPage > 0)
                        Logger?.Debug($"Accepted {acceptedFromPage}/{lastResult.SongsOnPage} songs from '{lastResult.Uri}'");
                    else
                        Logger?.Debug($"Zero songs on page '{lastResult.Uri}'");
                    if (acceptedSongs.Count >= maxSongs)
                        break;
                    if (resultsWithZeroSongs > 2)
                    {
                        Logger?.Warning($"More than two feed pages had zero songs on the page, this should've been handled earlier. Ending loop for safety.");
                        break;
                    }
                }

                FeedResult result = new FeedResult(acceptedSongs, pageResults);
                return result;
            }
            catch (Exception ex)
            {
                return new FeedResult(null, null,
                    new FeedReaderException($"An unhandled exception occurred reading feed '{FeedId}'.", ex, FeedReaderFailureCode.Generic),
                    FeedResultErrorLevel.Error);
            }
        }

        /// <inheritdoc/>
        public abstract FeedAsyncEnumerator GetAsyncEnumerator();

        /// <summary>
        /// Returns true if the settings are valid for this feed.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected abstract bool AreSettingsValid(IFeedSettings settings);

        /// <summary>
        /// Parses the given page text into a list of <see cref="ScrapedSong"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="InvalidFeedSettingsException"></exception>
        protected virtual PageReadResult ParseSongsFromPage(PageContent content, Uri uri)
        {
            EnsureValidSettings(); // EnsureValidSettings() checks for null.
            return PageHandler.Parse(content, uri, FeedSettings!);
        }

    }
}
