using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Handles reading content from a Beast Saber feed page.
    /// </summary>
    public class BeastSaberPageHandler : FeedPageHandlerBase, IBeastSaberPageHandler
    {
        private const string XML_TITLE_KEY = "SongTitle";
        private const string XML_DOWNLOADURL_KEY = "DownloadURL";
        private const string XML_HASH_KEY = "Hash";
        private const string XML_AUTHOR_KEY = "LevelAuthorName";
        private const string XML_SONGKEY_KEY = "SongKey";


        /// <summary>
        /// Creates a new <see cref="BeastSaberPageHandler"/>.
        /// </summary>
        public BeastSaberPageHandler() { }

        /// <summary>
        /// Creates a new <see cref="BeastSaberPageHandler"/> with a logger.
        /// </summary>
        /// <param name="logFactory"></param>
        public BeastSaberPageHandler(ILogFactory? logFactory)
            : base(logFactory)
        {
        }

        /// <inheritdoc/>
        public override PageReadResult Parse(PageContent content, Uri? pageUri, IFeedSettings settings)
        {
            if (string.IsNullOrWhiteSpace(content.Content))
            {
                return new PageReadResult(pageUri,
                    new PageParseException($"The page text was empty for '{pageUri}'"),
                    PageErrorType.SiteError);
            }
            try
            {
                List<ScrapedSong> songs;
                if (content.ContentId == PageContent.ContentId_JSON)
                    songs = ParseJsonPage(content.Content, pageUri, settings);
                else if (content.ContentId == PageContent.ContentId_XML)
                    songs = ParseXMLPage(content.Content, pageUri, settings);
                else
                {
                    Logger?.Warning($"Unrecognized ContentId ({content.ContentId}), trying JSON");
                    songs = ParseJsonPage(content.Content, pageUri, settings);
                }

                return CreateResult(songs, pageUri, settings);
            }
            catch (PageParseException ex)
            {
                return new PageReadResult(pageUri, ex, PageErrorType.ParsingError);
            }
            catch(Exception ex)
            {
                return new PageReadResult(pageUri, ex, PageErrorType.Unknown);
            }
        }

        /// <summary>
        /// Parses a list of <see cref="ScrapedSong"/> from the given JSON <paramref name="pageText"/>.
        /// </summary>
        /// <param name="pageText"></param>
        /// <param name="sourceUri"></param>
        /// <param name="settings"></param>
        /// <exception cref="PageParseException">Thrown when the page text is unable to parsed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pageText"/> or <paramref name="settings"/> is null.</exception>
        /// <returns></returns>
        public virtual List<ScrapedSong> ParseJsonPage(string pageText, Uri? sourceUri, IFeedSettings settings)
        {
            if (string.IsNullOrWhiteSpace(pageText))
                throw new ArgumentNullException(nameof(pageText));
            if (settings == null)
                throw new ArgumentNullException(nameof(pageText));
            var songsOnPage = new List<ScrapedSong>();
            JObject result;
            try
            {
                result = JObject.Parse(pageText);
            }
            catch (JsonReaderException ex)
            {
                throw new PageParseException($"Unable to parse JSON from '{sourceUri}'.", ex);
            }
            bool storeRawData = settings.StoreRawData;
            var songs = result["songs"] ?? throw new PageParseException($"Page at '{sourceUri}' did not contain a 'songs' property");

            foreach (JObject bSong in songs)
            {
                // Try to get the song hash from BeastSaber
                string? songHash = bSong["hash"]?.Value<string>();
                string? songKey = bSong["song_key"]?.Value<string>();
                string? songName = bSong["title"]?.Value<string>();
                string? mapperName = bSong["level_author_name"]?.Value<string>();
                Uri? downloadUri;

                if (songHash != null && songHash.Length > 0)
                {
                    downloadUri = BeatSaverHelper.GetDownloadUriByHash(songHash);
                    songsOnPage.Add(new ScrapedSong(songHash, songName, mapperName, downloadUri, sourceUri, storeRawData ? bSong : null) { Key = songKey });
                }
            }
            return songsOnPage;
        }

        /// <summary>
        /// Parses a list of <see cref="ScrapedSong"/> from the given XML <paramref name="pageText"/>.
        /// </summary>
        /// <param name="pageText"></param>
        /// <param name="sourceUri"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="PageParseException"></exception>
        public virtual List<ScrapedSong> ParseXMLPage(string pageText, Uri? sourceUri, IFeedSettings settings)
        {
            if (string.IsNullOrWhiteSpace(pageText))
                throw new ArgumentNullException(nameof(pageText));
            if (settings == null)
                throw new ArgumentNullException(nameof(pageText));
            bool retry = false;
            var songsOnPage = new List<ScrapedSong>();
            bool storeRawData = settings.StoreRawData;
            XmlDocument? xmlDocument = null;
            do
            {
                try
                {
                    xmlDocument = new XmlDocument() { XmlResolver = null };
                    var sr = new StringReader(pageText);
                    using (var reader = XmlReader.Create(sr, new XmlReaderSettings() { XmlResolver = null }))
                    {
                        xmlDocument.Load(reader);
                    }
                    retry = false;
                }
                catch (XmlException ex)
                {
                    if (retry == true)
                    {
                        throw new PageParseException($"Unable to parse XML page from '${sourceUri}'.", ex);
                    }
                    else
                    {
                        Logger?.Debug("Invalid XML formatting detected, attempting to fix...");
                        pageText = pageText.Replace(" & ", " &amp; ");
                        retry = true;
                    }
                    //File.WriteAllText("ErrorText.xml", pageText);
                }
            } while (retry == true);
            if (xmlDocument == null)
                throw new PageParseException($"xmlDocument was null for '{sourceUri}'.");
            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/rss/channel/item");
            int nodeCount = xmlNodeList.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode node = xmlNodeList[i];
                if (node["DownloadURL"] == null || node["SongTitle"] == null)
                {
                    Logger?.Debug($"XML node '{i}' is not a song! Skipping! ({sourceUri})");
                }
                else
                {
                    string? songName = node[XML_TITLE_KEY].InnerText;
                    string? downloadUrl = node[XML_DOWNLOADURL_KEY]?.InnerText;
                    string? hash = node[XML_HASH_KEY]?.InnerText?.ToUpper();
                    string? mapperName = node[XML_AUTHOR_KEY]?.InnerText;
                    string? songKey = node[XML_SONGKEY_KEY]?.InnerText;
                    if (hash == null || hash.Length == 0) // TODO: Could use Key if Hash was null.
                    {
                        Logger?.Warning($"Skipping BeastSaber song with null hash.");
                        continue;
                    }
                    if (downloadUrl?.Contains("dl.php") ?? true)
                    {
                        Logger?.Warning("Skipping BeastSaber download with old url format!");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hash))
                        {
                            JObject? jObject = null;
                            if (storeRawData)
                            {
                                jObject = new JObject
                                {
                                    { XML_TITLE_KEY, songName },
                                    { XML_DOWNLOADURL_KEY, downloadUrl },
                                    { XML_HASH_KEY, hash },
                                    { XML_AUTHOR_KEY, mapperName },
                                    { XML_SONGKEY_KEY, songKey }
                                };
                            }
                            if (!Uri.TryCreate(downloadUrl, UriKind.Absolute, out Uri? downloadUri))
                            {
                                Logger?.Debug($"Could not create a URI from '{downloadUrl}' with song '{songName} by {mapperName}.");
                                if (!string.IsNullOrWhiteSpace(hash))
                                    downloadUri = BeatSaverHelper.GetDownloadUriByHash(hash);
                                else
                                    downloadUri = null;
                            }
                            songsOnPage.Add(new ScrapedSong(hash, songName, mapperName, downloadUri, sourceUri, jObject) { Key = songKey });
                        }
                    }
                }
            }
            return songsOnPage;
        }


        /// <summary>
        /// Content type for a Beast Saber page.
        /// </summary>
        protected enum ContentType
        {
            /// <summary>
            /// Unknown content.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// XML content.
            /// </summary>
            XML = 1,
            /// <summary>
            /// JSON content.
            /// </summary>
            JSON = 2
        }
    }
}
