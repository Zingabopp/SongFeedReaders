using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Services.SongInfoProviders
{
    /// <summary>
    /// An <see cref="ISongInfoProvider"/> to request song information from Beat Saver.
    /// </summary>
    public class BeatSaverSongInfoProvider : SongInfoProviderBase
    {
        /// <summary>
        /// Web client used to make web requests.
        /// </summary>
        protected readonly IWebClient Client;
        /// <summary>
        /// Page handler to use to parse Beat Saver API pages.
        /// </summary>
        protected readonly IBeatSaverPageHandler PageHandler;
        /// <summary>
        /// Beat Saver base Uri to use.
        /// </summary>
        public Uri BeatSaverUri { get; private set; }
        /// <summary>
        /// Creates a new <see cref="BeatSaverSongInfoProvider"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pageHandler"></param>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BeatSaverSongInfoProvider(IWebClient client, IBeatSaverPageHandler pageHandler,
            ILogFactory? logFactory)
            : this(BeatSaverHelper.BeatSaverApiUri, client, pageHandler, logFactory)
        {

        }
        /// <summary>
        /// Creates a new <see cref="BeatSaverSongInfoProvider"/>.
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="client"></param>
        /// <param name="pageHandler"></param>
        /// <param name="logFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BeatSaverSongInfoProvider(Uri apiUri, IWebClient client, IBeatSaverPageHandler pageHandler,
            ILogFactory? logFactory)
            : base(logFactory)
        {
            BeatSaverUri = apiUri ?? throw new ArgumentNullException(nameof(apiUri));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            PageHandler = pageHandler ?? throw new ArgumentNullException(nameof(pageHandler));
        }

        /// <inheritdoc/>
        public override bool Available => true;

        /// <summary>
        /// Parses a <see cref="ScrapedSong"/> from a <see cref="Uri"/> to a Beat Saver song details page.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="SongInfoProviderException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<ScrapedSong?> GetSongFromUriAsync(Uri uri, CancellationToken cancellationToken)
        {
            IWebResponseMessage? response = null;
            try
            {
                response = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response.Content == null) return null;
                string pageText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return PageHandler.ParseSingle(pageText, uri, true);
            }
            catch (WebClientException ex)
            {
                string errorText = string.Empty;
                if (ex.Response != null)
                {
                    int statusCode = ex.Response.StatusCode;
                    if (statusCode == 404)
                        return null;
                    errorText = statusCode switch
                    {
                        408 => "Timeout",
                        _ => $"Site Error: {statusCode}",
                    };
                }
                string message = $"Exception getting page: '{uri}'";
                throw new SongInfoProviderException(message, ex);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                string message = $"Exception while trying to get details from '{uri}'";
                throw new OperationCanceledException(message, ae);
            }
            catch (Exception ex)
            {
                string message = $"Exception while trying to get details from '{uri}'";
                throw new SongInfoProviderException(message, ex);
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <inheritdoc/>
        public override Task<ScrapedSong?> GetSongByHashAsync(string hash, CancellationToken cancellationToken)
        {
            Uri uri = BeatSaverHelper.GetBeatSaverDetailsByHash(hash);
            return GetSongFromUriAsync(uri, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<ScrapedSong?> GetSongByKeyAsync(string key, CancellationToken cancellationToken)
        {
            Uri uri = BeatSaverHelper.GetBeatSaverDetailsByKey(key);
            return GetSongFromUriAsync(uri, cancellationToken);
        }
    }
}
