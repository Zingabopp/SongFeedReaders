using Newtonsoft.Json.Linq;
using System;

namespace SongFeedReaders.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ScrapedSong : ISong
    {
        /// <summary>
        /// Creates a new ScrapedSong using the Beat Saver key as an identifier.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="songName"></param>
        /// <param name="mapperName"></param>
        /// <param name="downloadUri"></param>
        /// <param name="sourceUri"></param>
        /// <param name="jsonData"></param>
        /// <param name="songHash"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null or empty.</exception>
        public static ScrapedSong CreateFromKey(string key, string songName, string mapperName,
            Uri downloadUri, Uri sourceUri, JObject? jsonData = null, string? songHash = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} cannot be null for ScrapedSong.CreateFromKey.");
            return new ScrapedSong()
            {
                Key = key,
                Name = songName,
                LevelAuthorName = mapperName,
                DownloadUri = downloadUri,
                SourceUri = sourceUri,
                JsonData = jsonData,
                Hash = songHash
            };
        }

        private string? _hash;
        /// <summary>
        /// Beat Saver hash of the song, always uppercase.
        /// </summary>
        public string? Hash
        {
            get { return _hash; }
            set { _hash = value?.ToUpper(); }
        }
        /// <summary>
        /// Full URL to download song.
        /// </summary>
        public Uri? DownloadUri { get; set; }
        /// <summary>
        /// What web page this song was scraped from.
        /// </summary>
        public Uri? SourceUri { get; set; }
        /// <inheritdoc/>
        public string? Name { get; set; }
        private string? _songKey;
        /// <summary>
        /// Beat Saver song key, always uppercase.
        /// </summary>
        public string? Key
        {
            get { return _songKey; }
            set { _songKey = value?.ToUpper(); }
        }
        /// <inheritdoc/>
        public string? LevelAuthorName { get; set; }

        /// <summary>
        /// Upload date of the song. Uses <see cref="DateTime.MinValue"/> if not set;
        /// </summary>
        public DateTime UploadDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Data this song was scraped from in JSON form.
        /// </summary>
        public string? RawData => JsonData?.ToString();
        /// <summary>
        /// Raw JSON data for the song.
        /// </summary>
        public JObject? JsonData { get; set; }

        /// <summary>
        /// Creates an empty <see cref="ScrapedSong"/>.
        /// </summary>
        public ScrapedSong() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is null or empty.</exception>
        public ScrapedSong(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentNullException(nameof(hash), $"{nameof(hash)} cannot be null for ScrapedSong(string).");
            Hash = hash;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="songName"></param>
        /// <param name="mapperName"></param>
        /// 
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is null or empty.</exception>
        public ScrapedSong(string hash, string? songName, string? mapperName)
            : this(hash)
        {
            Name = songName;
            LevelAuthorName = mapperName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="songName"></param>
        /// <param name="mapperName"></param>
        /// <param name="songKey"></param>
        public ScrapedSong(string hash, string? songName, string? mapperName, string? songKey)
           : this(hash, songName, mapperName)
        {
            Key = songKey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="songName"></param>
        /// <param name="mapperName"></param>
        /// <param name="downloadUri"></param>
        /// <param name="sourceUri"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is null or empty.</exception>
        public ScrapedSong(string hash, string? songName, string? mapperName, Uri? downloadUri, Uri? sourceUri)
            : this(hash, songName, mapperName)
        {
            DownloadUri = downloadUri;
            SourceUri = sourceUri;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="songName"></param>
        /// <param name="mapperName"></param>
        /// <param name="downloadUri"></param>
        /// <param name="sourceUri"></param>
        /// <param name="jsonData"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash"/> is null or empty.</exception>
        public ScrapedSong(string hash, string? songName, string? mapperName, Uri? downloadUri, Uri? sourceUri, JObject? jsonData)
            : this(hash, songName, mapperName, downloadUri, sourceUri)
        {
            JsonData = jsonData;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string keyStr;
            if (!string.IsNullOrEmpty(Key))
                keyStr = $"({Key}) ";
            else
                keyStr = string.Empty;
            return $"{keyStr}{Name} by {LevelAuthorName}";
        }

        /// <summary>
        /// Updates this object with information from <paramref name="other"/>.
        /// Only updates data from <paramref name="other"/> if it exists (does not copy null data).
        /// </summary>
        /// <param name="other"></param>
        /// <param name="updateJson"></param>
        public virtual void UpdateFrom(ScrapedSong other, bool updateJson = true)
        {
            if (!string.IsNullOrWhiteSpace(other.Hash))
                Hash = other.Hash;
            if (DownloadUri != null)
                DownloadUri = other.DownloadUri;
            if (!string.IsNullOrWhiteSpace(other.Key))
                Key = other.Key;
            if (!string.IsNullOrWhiteSpace(other.Name))
                Name = other.Name;
            if (!string.IsNullOrWhiteSpace(other.LevelAuthorName))
                Name = other.LevelAuthorName;
            if (other.UploadDate > UploadDate)
                UploadDate = other.UploadDate;
            if (updateJson)
                JsonData = other.JsonData;
        }
        ///// <summary>
        ///// Copies the values from another ScrapedSong into this one.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentNullException">Thrown when the other song is null.</exception>
        //public ScrapedSong UpdateFrom(ScrapedSong other, bool changeSourceUri = true)
        //{
        //    if (other == null)
        //        throw new ArgumentNullException(nameof(other), "Other song cannot be null for ScrapedSong.UpdateFrom.");
        //    Hash = other.Hash;
        //    DownloadUri = other.DownloadUri;
        //    if(changeSourceUri)
        //        SourceUri = other.SourceUri;
        //    SongName = other.SongName;
        //    SongKey = other.SongKey;
        //    MapperName = other.MapperName;
        //    JsonData = other.JsonData;
        //    return this;
        //}
    }
}
