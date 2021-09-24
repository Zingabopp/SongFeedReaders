using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// Handles parsing the page content from ScoreSaber.
    /// </summary>
    public class ScoreSaberPageHandler : FeedPageHandlerBase, IScoreSaberPageHandler
    {
        /// <inheritdoc/>
        public override PageReadResult Parse(PageContent content, Uri? pageUri, IFeedSettings settings)
        {
            JObject result;
            List<ScrapedSong> songsOnPage = new List<ScrapedSong>();
            try
            {
                result = JObject.Parse(content.Content);
            }
            catch (JsonReaderException ex)
            {
                throw new PageParseException($"Error reading JSON from page '{pageUri}'", ex);
            }
            JToken[]? songJSONAry = result["songs"]?.ToArray();
            if (songJSONAry == null)
            {
                string message = "Invalid page text: 'songs' field not found.";
                throw new PageParseException(message);
            }
            foreach (JObject jSong in songJSONAry)
            {
                string? hash = jSong["id"]?.Value<string>();
                if (hash == null || hash.Length == 0)
                {
                    // TODO: Log something or throw here?
                    continue;
                }
                string? songName = jSong["name"]?.Value<string>();
                string? mapperName = jSong["levelAuthorName"]?.Value<string>();
                ScrapedSong song = new ScrapedSong(
                    hash: hash,
                    songName: songName,
                    mapperName: mapperName,
                    downloadUri: BeatSaverHelper.GetDownloadUriByHash(hash),
                    sourceUri: pageUri,
                    jsonData: settings.StoreRawData ? jSong : null);
                if (!string.IsNullOrEmpty(hash))
                    songsOnPage.Add(song);
            }
            return CreateResult(songsOnPage, pageUri, settings);
        }
    }
}
