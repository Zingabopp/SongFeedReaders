using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services.SongInfoProviders
{
    /// <summary>
    /// Base class for an <see cref="ISongInfoProvider"/>.
    /// </summary>
    public abstract class SongInfoProviderBase : ISongInfoProvider
    {
        /// <summary>
        /// Logger used by this instance.
        /// </summary>
        protected readonly ILogger? Logger;

        /// <inheritdoc/>
        public int Priority { get; set; }

        /// <inheritdoc/>
        public abstract bool Available { get; }


        /// <summary>
        /// Initializes a new <see cref="SongInfoProviderBase"/> with an <see cref="ILogFactory"/>.
        /// </summary>
        /// <param name="logFactory"></param>
        protected SongInfoProviderBase(ILogFactory? logFactory)
        {
            Logger = logFactory?.GetLogger(GetType().Name);
        }

        /// <inheritdoc/>
        public abstract Task<ScrapedSong?> GetSongByHashAsync(string hash, CancellationToken cancellationToken);

        /// <inheritdoc/>

        public abstract Task<ScrapedSong?> GetSongByKeyAsync(string key, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public Task<ScrapedSong?> GetSongByHashAsync(string hash) => GetSongByHashAsync(hash, CancellationToken.None);

        /// <inheritdoc/>
        public Task<ScrapedSong?> GetSongByKeyAsync(string key) => GetSongByKeyAsync(key, CancellationToken.None);
    }
}
