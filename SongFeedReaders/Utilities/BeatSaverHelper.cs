using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Utilities
{
    /// <summary>
    /// Helpers for interacting with Beat Saver.
    /// </summary>
    public static class BeatSaverHelper
    {
        static BeatSaverHelper()
        {
            BeatSaverUri = new Uri("https://beatsaver.com");
        }

        private static Uri beatSaverUri = null!;
        private static Uri beatSaverApiUri = null!;
        private static Uri beatSaverDownloadUri = null!;
        private static Uri beatSaverDetailsFromKeyBaseUrl = null!;
        private static Uri beatSaverDetailsFromHashBaseUrl = null!;
        /// <summary>
        /// Beat Saver's URI. Default is <see href="https://beatsaver.com"/>.
        /// </summary>
        public static Uri BeatSaverUri
        {
            get => beatSaverUri;
            set
            {
                if (value != beatSaverUri && value != null)
                {
                    beatSaverUri = value;
                    beatSaverApiUri = new Uri($"{value.Scheme}://api.{value.Host}", UriKind.Absolute);
                    beatSaverDownloadUri = new Uri($"{value.Scheme}://cdn.{value.Host}", UriKind.Absolute);
                    beatSaverDetailsFromKeyBaseUrl = new Uri(beatSaverApiUri, "/maps/beatsaver/");
                    beatSaverDetailsFromHashBaseUrl = new Uri(beatSaverApiUri, "/maps/hash/");
                }
            }
        }
        /// <summary>
        /// URI for Beat Saver's API.
        /// </summary>
        public static Uri BeatSaverApiUri => beatSaverApiUri;
        /// <summary>
        /// Base URI for Beat Saver downloads.
        /// </summary>
        public static Uri BeatSaverDownloadUri => beatSaverDownloadUri;
        /// <summary>
        /// Base URI for Beat Saver song details using a key identifier.
        /// </summary>
        public static Uri BeatSaverDetailsFromKeyBaseUrl => beatSaverDetailsFromKeyBaseUrl;
        /// <summary>
        /// Base URI for Beat Saver song details using a hash identifier.
        /// </summary>
        public static Uri BeatSaverDetailsFromHashBaseUrl => beatSaverDetailsFromHashBaseUrl;

        /// <summary>
        /// Returns a Beat Saver beatmap download URI using the beatmap's hash.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Uri GetDownloadUriByHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentNullException(nameof(hash));
            return new Uri(BeatSaverDownloadUri, hash.ToLower() + ".zip");
        }

        /// <summary>
        /// Returns a URI for requesting a beatmap's details by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Uri GetBeatSaverDetailsByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return new Uri(BeatSaverDetailsFromKeyBaseUrl + key.ToLower());
        }

        /// <summary>
        /// Returns a URI for requesting a beatmap's details by hash.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Uri GetBeatSaverDetailsByHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentNullException(nameof(hash));
            return new Uri(BeatSaverDetailsFromHashBaseUrl + hash.ToLower());
        }
    }
}
