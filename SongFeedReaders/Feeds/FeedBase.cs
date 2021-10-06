using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Base class for an <see cref="IFeed"/>.
    /// </summary>
    public abstract class FeedBase : IFeed
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
        public virtual bool Initialized => true;
        /// <inheritdoc/>
        public IFeedSettings FeedSettings { get; set; }
        /// <inheritdoc/>
        public bool HasValidSettings => AreSettingsValid(FeedSettings);
        /// <inheritdoc/>
        public abstract void EnsureValidSettings();

        /// <summary>
        /// Initializes a new <see cref="FeedBase"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected FeedBase(IFeedSettings feedSettings, IFeedPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
        {
            FeedSettings = feedSettings ?? throw new ArgumentNullException(nameof(pageHandler));
            PageHandler = pageHandler ?? throw new ArgumentNullException(nameof(pageHandler));
            WebClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
            Logger = logFactory?.GetLogger(GetType().Name);
        }

        /// <summary>
        /// Initializes a new <see cref="FeedBase"/>.
        /// </summary>
        /// <param name="settingsFactory"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected FeedBase(ISettingsFactory settingsFactory, IFeedPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory = null)
        {
            FeedSettings = settingsFactory.GetSettings(FeedId) ?? throw new ArgumentException($"Settings factory doesn't have settings registered with feed ID '{FeedId}'");
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
            => CompletedInitialization;

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
        public virtual Task<FeedResult> ReadAsync(CancellationToken cancellationToken)
            => ReadAsync(null, cancellationToken);

        /// <inheritdoc/>
        public virtual async Task<FeedResult> ReadAsync(IProgress<PageReadResult>? progress, CancellationToken cancellationToken)
        {
            IFeedSettings settings = FeedSettings;
            EnsureValidSettings();
            try
            {
                if (!Initialized)
                {
                    await InitializeAsync(cancellationToken).ConfigureAwait(false);
                }
                FeedAsyncEnumerator asyncEnumerator = GetAsyncEnumerator(settings);
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
                    foreach (var song in lastResult.Songs())
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
        public abstract FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings);

        /// <inheritdoc/>
        public virtual FeedAsyncEnumerator GetAsyncEnumerator()
        {
            EnsureValidSettings();
            return GetAsyncEnumerator(FeedSettings);
        }

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
            EnsureValidSettings();
            return PageHandler.Parse(content, uri, FeedSettings);
        }

    }
}
