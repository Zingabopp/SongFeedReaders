using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// Handles parsing the page content from ScoreSaber.
    /// </summary>
    public class ScoreSaberPageHandler : FeedPageHandlerBase, IScoreSaberPageHandler
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberPageHandler"/>.
        /// </summary>
        public ScoreSaberPageHandler()
        { }
        /// <summary>
        /// Creates a new <see cref="ScoreSaberPageHandler"/>.
        /// </summary>
        /// <param name="logFactory"></param>
        public ScoreSaberPageHandler(ILogFactory? logFactory)
            : base(logFactory)
        {
        }

        /// <inheritdoc/>
        public override PageReadResult Parse(PageContent content, Uri? pageUri, IFeedSettings settings)
        {
            JObject json;
            List<ScrapedSong> songsOnPage = new List<ScrapedSong>();
            try
            {
                json = JObject.Parse(content.Content);
            }
            catch (JsonReaderException ex)
            {
                throw new PageParseException($"Error reading JSON from page '{pageUri}'", ex);
            }
            JToken[]? songJSONAry = json["songs"]?.ToArray();
            if (songJSONAry == null)
            {
                string message = "Invalid page text: 'songs' field not found.";
                throw new PageParseException(message);
            }
            int songCount = 0;
            foreach (JObject jSong in songJSONAry)
            {
                songCount++;
                ScrapedSong song = CreateSong(jSong, settings.StoreRawData);
                song.SourceUri = pageUri;
                string? hash = song.Hash;
                if (hash == null || hash.Length == 0)
                {
                    // TODO: Log something or throw here?
                    continue;
                }
                if (!string.IsNullOrEmpty(hash))
                    songsOnPage.Add(song);
            }
            bool isLastPage = songCount < settings.SongsPerPage;
            PageReadResult result = CreateResult(songsOnPage, pageUri, settings, isLastPage);
            return result;
        }

        /// <summary>
        /// Creates a <see cref="ScrapedSong"/> from a ScoreSaber <see cref="JObject"/>.
        /// </summary>
        /// <param name="jSong"></param>
        /// <param name="storeRawData"></param>
        /// <returns></returns>
        public static ScrapedSong CreateSong(JObject jSong, bool storeRawData)
        {
            ScrapedSong? song = jSong.ToObject<ScoreSaberSong>()
                ?? new ScrapedSong();
            string? hash = jSong["id"]?.Value<string>();
            song.Hash = hash;

            song.Name = jSong["name"]?.Value<string>();
            song.LevelAuthorName = jSong["levelAuthorName"]?.Value<string>();
            if (!string.IsNullOrWhiteSpace(hash))
                song.DownloadUri = BeatSaverHelper.GetDownloadUriByHash(hash!);
            if (storeRawData)
                song.JsonData = jSong;
            return song;
        }
    }
}
