using SongFeedReaders.Logging;
using SongFeedReaders.Models;
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
        public abstract string DisplayName { get; }
        /// <inheritdoc/>
        public abstract string Description { get; }
        /// <inheritdoc/>
        public IFeedSettings FeedSettings { get; protected set; }
        /// <inheritdoc/>
        public bool HasValidSettings => AreSettingsValid(FeedSettings);
        /// <inheritdoc/>
        public void EnsureValidSettings()
        {
            if (!AreSettingsValid(FeedSettings))
                throw new InvalidFeedSettingsException();
        }


        /// <summary>
        /// Initializes a new <see cref="FeedBase"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        protected FeedBase(IFeedSettings feedSettings, IFeedPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
        {
            FeedSettings = feedSettings?.Clone() as IFeedSettings ?? throw new ArgumentNullException(nameof(feedSettings));
            PageHandler = pageHandler ?? throw new ArgumentNullException(nameof(pageHandler));
            WebClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
            Logger = logFactory?.GetLogger();
        }
        /// <summary>
        /// Returns a <see cref="PageContent"/> from <paramref name="responseContent"/>.
        /// </summary>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        protected abstract Task<PageContent> GetPageContent(IWebResponseContent responseContent);

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
                List<ScrapedSong> pageSongs = ParseSongsFromPage(pageContent, uri);

                return CreateResult(uri, pageSongs);
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
            FeedAsyncEnumerator asyncEnumerator = GetAsyncEnumerator(settings);
            List<PageReadResult> pageResults = new List<PageReadResult>();
            int songCount = 0;
            PageReadResult? lastResult = null;
            while (asyncEnumerator.CanMoveNext && !(lastResult?.IsLastPage ?? false))
            {
                lastResult = await asyncEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
                songCount += lastResult.SongCount;
                pageResults.Add(lastResult);
                // TODO: Repeated pages with no songs should break the loop.
                progress?.Report(lastResult);
                if (!lastResult.Successful)
                {
                    return new FeedResult(pageResults, settings);
                }
                if (settings.MaxSongs > 0 && songCount >= settings.MaxSongs)
                    break;
            }
            FeedResult result = new FeedResult(pageResults, settings);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="FeedAsyncEnumerator"/> for this feed.
        /// </summary>
        /// <returns></returns>
        protected abstract FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings);

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
        protected virtual List<ScrapedSong> ParseSongsFromPage(PageContent content, Uri uri)
            => PageHandler.Parse(content, uri, FeedSettings);

        /// <summary>
        /// Creates a <see cref="PageReadResult"/> from a collection of songs.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pageSongs"></param>
        /// <returns></returns>
        protected virtual PageReadResult CreateResult(Uri uri, IEnumerable<ScrapedSong> pageSongs)
        {
            bool isLastPage = false;
            Func<ScrapedSong, bool>? stopWhenAny = FeedSettings.StopWhenAny;
            Func<ScrapedSong, bool>? filter = FeedSettings.Filter;
            int totalSongs = pageSongs.Count();
            List<ScrapedSong> songs = new List<ScrapedSong>(totalSongs);
            foreach (ScrapedSong? song in pageSongs)
            {
                if (stopWhenAny != null && stopWhenAny(song))
                {
                    isLastPage = true;
                    break;
                }
                // TODO: filter could throw exception here.
                if (filter == null || filter(song))
                    songs.Add(song);
            }

            // TODO: Check more things for isLastPage?
            return new PageReadResult(
                uri: uri,
                songs: songs,
                firstSong: pageSongs.FirstOrDefault(),
                lastSong: pageSongs.LastOrDefault(),
                songsOnPage: totalSongs,
                isLastPage: isLastPage);

        }
    }
}
