using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// A collection that holds and organizes <see cref="ISongInfoProvider"/>s.
    /// </summary>
    public interface ISongInfoManager
    {
        /// <summary>
        /// Attempts to retreive the song information for the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="SongInfoManager"/> has no providers.</exception>
        Task<SongInfoResponse> GetSongByHashAsync(string hash, CancellationToken cancellationToken);
        /// <summary>
        /// Attempts to retreive the song information for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="SongInfoManager"/> has no providers.</exception>
        Task<SongInfoResponse> GetSongByKeyAsync(string key, CancellationToken cancellationToken);
        /// <summary>
        /// Adds a <see cref="ISongInfoProvider"/> to the manager.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="providerId"></param>
        /// <param name="priority"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        void AddProvider(ISongInfoProvider provider, string providerId, int priority = 100);
        /// <summary>
        /// Adds a <see cref="ISongInfoProvider"/> to the manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="providerId"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public T AddProvider<T>(string providerId, int priority = 100)
            where T : ISongInfoProvider, new();
    }
}
