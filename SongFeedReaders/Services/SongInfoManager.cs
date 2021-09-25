using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// A collection that holds and organizes <see cref="ISongInfoProvider"/>s.
    /// </summary>
    public sealed class SongInfoManager : ISongInfoManager
    {
        private readonly List<InfoProviderEntry> InfoProviders = new List<InfoProviderEntry>();
        private readonly object _providerLock = new object();

        /// <inheritdoc/>
        public async Task<SongInfoResponse> GetSongByHashAsync(string hash, CancellationToken cancellationToken)
        {
            InfoProviderEntry[] infoProviders = GetProvidersEntries();
            if (infoProviders.Length == 0) throw new InvalidOperationException("SongInfoManager has no providers.");
            List<SongInfoResponse> failedResponses = new List<SongInfoResponse>();
            SongInfoResponse? lastResponse = null;
            for (int i = 0; i < infoProviders.Length; i++)
            {
                try
                {
                    ScrapedSong? song = await infoProviders[i].InfoProvider.GetSongByHashAsync(hash, cancellationToken).ConfigureAwait(false);
                    lastResponse = new SongInfoResponse(song, infoProviders[i].InfoProvider);
                }
                catch (Exception ex)
                {
                    lastResponse = new SongInfoResponse(null, infoProviders[i].InfoProvider, ex);
                }
                if (lastResponse.Success)
                {
                    break;
                }
                else
                    failedResponses.Add(lastResponse);
            }
            if (lastResponse != null)
            {
                if (failedResponses.Count > 0)
                    lastResponse.FailedResponses = failedResponses.ToArray();
                return lastResponse;
            }
            else
                return SongInfoResponse.FailedResponse;
        }

        /// <inheritdoc/>
        public async Task<SongInfoResponse> GetSongByKeyAsync(string key, CancellationToken cancellationToken)
        {
            InfoProviderEntry[] infoProviders = GetProvidersEntries();
            if (infoProviders.Length == 0) throw new InvalidOperationException("SongInfoManager has no providers.");
            List<SongInfoResponse> failedResponses = new List<SongInfoResponse>();
            SongInfoResponse? lastResponse = null;
            for (int i = 0; i < infoProviders.Length; i++)
            {
                try
                {
                    ScrapedSong? song = await infoProviders[i].InfoProvider.GetSongByKeyAsync(key, cancellationToken).ConfigureAwait(false);
                    lastResponse = new SongInfoResponse(song, infoProviders[i].InfoProvider);
                }
                catch (Exception ex)
                {
                    lastResponse = new SongInfoResponse(null, infoProviders[i].InfoProvider, ex);
                }
                if (lastResponse.Success)
                {
                    break;
                }
                else
                    failedResponses.Add(lastResponse);
            }
            if (lastResponse != null)
            {
                if (failedResponses.Count > 0)
                    lastResponse.FailedResponses = failedResponses.ToArray();
                return lastResponse;
            }
            else
                return SongInfoResponse.FailedResponse;
        }

        /// <inheritdoc/>
        public void AddProvider(ISongInfoProvider provider, string providerId, int priority = 100)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(providerId))
                throw new ArgumentNullException(nameof(providerId));
            lock (_providerLock)
            {
                if (InfoProviders.Any(p => p.ProviderID == providerId))
                    throw new ArgumentException($"Provider with ID '{providerId} already exists.");
                InfoProviders.Add(new InfoProviderEntry(provider, providerId) { Priority = priority });
            }
        }

        /// <inheritdoc/>
        public T AddProvider<T>(string providerId, int priority = 100)
            where T : ISongInfoProvider, new()
        {
            T provider = new T();
            AddProvider(provider, providerId, priority);
            return provider;
        }

        /// <summary>
        /// Removes a provider from the manager.
        /// </summary>
        /// <param name="infoProviderEntry"></param>
        /// <returns></returns>
        public bool RemoveProvider(InfoProviderEntry infoProviderEntry)
        {
            lock (_providerLock)
            {
                return InfoProviders.Remove(infoProviderEntry);
            }

        }
        /// <summary>
        /// Removes a provider from the manager by <paramref name="providerId"/>.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public bool RemoveProvider(string providerId)
        {
            InfoProviderEntry? entry = GetProviderEntry(providerId);
            if (entry != null)
                return RemoveProvider(entry);
            else
                return false;
        }
        /// <summary>
        /// Gets an array of all <see cref="InfoProviderEntry"/> in the manager 
        /// in descending order by priority.
        /// </summary>
        /// <returns></returns>
        public InfoProviderEntry[] GetProvidersEntries()
        {
            lock (_providerLock)
            {
                return InfoProviders.OrderByDescending(e => e.Priority).ToArray();
            }
        }
        /// <summary>
        /// Gets an <see cref="InfoProviderEntry"/> by the given <paramref name="providerId"/>.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public InfoProviderEntry? GetProviderEntry(string providerId)
        {
            lock (_providerLock)
            {
                return InfoProviders.FirstOrDefault(p => p.ProviderID == providerId);
            }

        }
        /// <summary>
        /// Contains a <see cref="ISongInfoProvider"/> and some metadata for the <see cref="SongInfoManager"/>.
        /// </summary>
        public sealed class InfoProviderEntry
        {
            /// <summary>
            /// Creates a new <see cref="InfoProviderEntry"/>
            /// </summary>
            /// <param name="infoProvider"></param>
            /// <param name="providerId"></param>
            public InfoProviderEntry(ISongInfoProvider infoProvider, string providerId)
            {
                InfoProvider = infoProvider;
                ProviderID = providerId;
            }
            /// <summary>
            /// The <see cref="ISongInfoProvider"/>.
            /// </summary>
            public ISongInfoProvider InfoProvider { get; private set; }
            /// <summary>
            /// Priority of this <see cref="InfoProvider"/>.
            /// Highest priorities are queried first.
            /// </summary>
            public int Priority { get; set; }
            /// <summary>
            /// Provider ID of this <see cref="InfoProvider"/>.
            /// </summary>
            public string ProviderID { get; set; }
        }
    }
}
