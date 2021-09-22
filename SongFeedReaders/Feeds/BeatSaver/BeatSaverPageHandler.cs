using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Handles parsing the page content from Beat Saver.
    /// </summary>
    public class BeatSaverPageHandler : IBeatSaverPageHandler
    {
        /// <summary>
        /// Logger used by this instance.
        /// </summary>
        protected readonly ILogger? Logger;
        /// <summary>
        /// Creates a new <see cref="BeatSaverPageHandler"/>.
        /// </summary>
        public BeatSaverPageHandler() { }
        /// <summary>
        /// Creates a new <see cref="BeatSaverPageHandler"/> with a logger.
        /// </summary>
        /// <param name="logFactory"></param>
        public BeatSaverPageHandler(ILogFactory? logFactory)
        {
            Logger = logFactory?.GetLogger();
        }

        /// <inheritdoc/>
        public List<ScrapedSong> Parse(string content, Uri? pageUri, bool storeRawData)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));
            try
            {
                JObject? jObj = JObject.Parse(content);
                return ParseSongsFromJson(jObj, pageUri, storeRawData);
            }
            catch (PageParseException)
            {
                throw;
            }
            catch(JsonReaderException ex)
            {
                string message = $"Failed to parse JSON from '{pageUri}'";
                Logger?.Debug(message);
                throw new PageParseException(message, ex);
            }
            catch(Exception ex)
            {
                throw new PageParseException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Parses out a List of ScrapedSongs from the json. Also works if the page is for a single song.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sourceUri"></param>
        /// <param name="storeRawData"></param>
        /// <returns></returns>
        public List<ScrapedSong> ParseSongsFromJson(JToken result, Uri? sourceUri, bool storeRawData)
        {

            List<ScrapedSong> songs = new List<ScrapedSong>();
            ScrapedSong newSong;

            // Single song in page text.
            if (result["docs"] == null && result.Type != JTokenType.Array)
            {
                if (result["id"] != null)
                {
                    newSong = ParseSongFromJson(result, sourceUri, storeRawData);
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
                throw new PageParseException($"Page content of '{sourceUri}' is unrecognized.");
            }

            foreach (JObject song in songJSONAry)
            {
                newSong = ParseSongFromJson(song, sourceUri, storeRawData);
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
                JObject latestVersion = GetLatestSongVersion(song);
                string? songKey = song["id"]?.Value<string>();
                string? songHash = latestVersion["hash"]?.Value<string>().ToUpper();
                string? songName = song["metadata"]?["songName"]?.Value<string>();
                string? mapperName = song["uploader"]?["name"]?.Value<string>();
                DateTime uploadDate = song["uploaded"]?.Value<DateTime>() ?? DateTime.MinValue;
                if (songHash == null || songHash.Length == 0)
                    throw new ArgumentException("Unable to find hash for the provided song, is this a valid song JObject?");
                Uri downloadUri;
                if (!Uri.TryCreate(latestVersion["downloadURL"]?.Value<string>(), UriKind.Absolute, out downloadUri))
                {
                    Logger?.Debug($"Failed to get download URI from JSON, calculating using hash.");
                    downloadUri = BeatSaverHelper.GetDownloadUriByHash(songHash);
                }
                ScrapedSong newSong = new ScrapedSong(
                    hash: songHash, 
                    songName: songName,
                    mapperName: mapperName, 
                    downloadUri: downloadUri,
                    sourceUri: sourceUri, 
                    jsonData: storeRawData ? song as JObject : null)
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Thrown if the song doesn't have any published versions available.</exception>
        public static JObject GetLatestSongVersion(JToken song)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null for BeatSaverReader.ParseSongFromJson.");
            if (song["versions"] is JArray versions && versions.Count > 0)
            {
                // take latest version
                if (versions.Where(VersionIsPublished).LastOrDefault() is JObject latest)
                    return latest;
                throw new ArgumentException("Song has no published versions.");
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