using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services.SongInfoProviders
{
    public abstract class SongInfoProviderBase : ISongInfoProvider
    {
        protected readonly ILogger? Logger;
        public int Priority { get; set; }

        public abstract bool Available { get; }



        protected SongInfoProviderBase(ILogFactory? logFactory)
        {
            Logger = logFactory?.GetLogger();
        } 

        public abstract Task<ScrapedSong?> GetSongByHashAsync(string hash, CancellationToken cancellationToken);


        public abstract Task<ScrapedSong?> GetSongByKeyAsync(string key, CancellationToken cancellationToken);

        public Task<ScrapedSong?> GetSongByHashAsync(string hash) => GetSongByHashAsync(hash, CancellationToken.None);

        public Task<ScrapedSong?> GetSongByKeyAsync(string key) => GetSongByKeyAsync(key, CancellationToken.None);
    }
}
