using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public abstract class BeatSaverFeed : FeedBase
    {
        /// <summary>
        /// Initializes a new <see cref="BeatSaverFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        protected BeatSaverFeed(IFeedSettings feedSettings, IFeedPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }

        /// <summary>
        /// Parses out a List of ScrapedSongs from the given page text. Also works if the page is for a single song.
        /// </summary>
        /// <param name="pageText"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        /// <exception cref="PageParseException"></exception>
        protected override List<ScrapedSong> ParseSongsFromPage(string pageText, Uri? sourceUrl)
        {
            JObject? result;
            try
            {
                result = JObject.Parse(pageText) ?? new JObject();
                return ParseSongsFromJson(result, sourceUrl, FeedSettings.StoreRawData);
            }
            catch (JsonReaderException ex)
            {
                string message = $"Failed to parse JSON from '{sourceUrl}'";
                Logger?.Debug(message);
                throw new PageParseException(message, ex);
            }
        }

        /// <summary>
        /// Parses out a List of ScrapedSongs from the json. Also works if the page is for a single song.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="storeRawData"></param>
        /// <returns></returns>
        public List<ScrapedSong> ParseSongsFromJson(JToken result, Uri? sourceUrl, bool storeRawData)
        {

            List<ScrapedSong> songs = new List<ScrapedSong>();
            ScrapedSong newSong;

            // Single song in page text.
            if (result["docs"] == null && result.Type != JTokenType.Array)
            {
                if (result["id"] != null)
                {
                    newSong = ParseSongFromJson(result, sourceUrl, storeRawData);
                    if (newSong != null)
                    {
                        songs.Add(newSong);
                        return songs;
                    }
                }
                return songs;
            }

            // Array of songs in page text.
            JToken[]? songJSONAry = result["docs"]?.ToArray();

            if (songJSONAry == null)
            {
                string message = "Invalid page text: 'docs' field not found.";
                Logger?.Error(message);
                throw new PageParseException(message);
            }

            foreach (JObject song in songJSONAry)
            {
                newSong = ParseSongFromJson(song, sourceUrl, storeRawData);
                if (newSong != null)
                    songs.Add(newSong);
            }
            return songs;
        }

        /// <summary>
        /// Creates a SongInfo from a JObject.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="sourceUri"></param>
        /// <param name="storeRawData"></param>
        /// <exception cref="ArgumentException">Thrown when a hash can't be found for the given song JObject.</exception>
        /// <returns></returns>
        public ScrapedSong ParseSongFromJson(JToken song, Uri? sourceUri, bool storeRawData)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null for BeatSaverReader.ParseSongFromJson.");
            if (song["versions"] is JArray versions && versions.Count > 0)
            {
                JObject latest = GetLatestSongVersion(song);
                //JSONObject song = (JSONObject) aKeyValue;
                string? songKey = song["id"]?.Value<string>();
                string? songHash = latest["hash"]?.Value<string>().ToUpper();
                string? songName = song["metadata"]?["songName"]?.Value<string>();
                string? mapperName = song["uploader"]?["name"]?.Value<string>();
                DateTime uploadDate = song["uploaded"]?.Value<DateTime>() ?? DateTime.MinValue;
                if (songHash == null || songHash.Length == 0)
                    throw new ArgumentException("Unable to find hash for the provided song, is this a valid song JObject?");
                Uri downloadUri;
                if (!Uri.TryCreate(latest["downloadURL"]?.Value<string>(), UriKind.Absolute, out downloadUri))
                {
                    Logger?.Debug($"Failed to get download URI from JSON, calculating using hash.");
                    downloadUri = BeatSaverHelper.GetDownloadUriByHash(songHash);
                }

                ScrapedSong newSong = new ScrapedSong(songHash, songName, mapperName, downloadUri, sourceUri, storeRawData ? song as JObject : null)
                {
                    Key = songKey,
                    UploadDate = uploadDate
                };
                return newSong;
            }
            else
                throw new ArgumentException("Song does not appear to have any versions available.", nameof(song));
        }

        /// <summary>
        /// Parses the latest version from a Beat Saver song's JSON.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public static JObject GetLatestSongVersion(JToken song)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null for BeatSaverReader.ParseSongFromJson.");
            if (song["versions"] is JArray versions && versions.Count > 0)
            {
                // take latest version
                if (versions.Where(VersionIsPublished).LastOrDefault() is JObject latest)
                    return latest;
                throw new Exception("Song has no published versions.");
            }
            else
                throw new ArgumentException("Song does not appear to have any versions available.", nameof(song));
        }
        private static bool VersionIsPublished(JToken v)
        {
            JToken? state = v["state"];
            return state != null && state.Value<string>().Equals("Published", StringComparison.OrdinalIgnoreCase);
        }
    }
}
