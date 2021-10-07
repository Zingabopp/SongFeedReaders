using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SongFeedReaders.Models
{
    /// <summary>
    /// Holds the data for a ScoreSaber song.
    /// </summary>
    public class ScoreSaberSong : ScrapedSong
    {
        /// <summary>
        /// Creates a new <see cref="ScoreSaberSong"/>.
        /// </summary>
        public ScoreSaberSong()
        {
        }

        /// <summary>
        /// Creates a new <see cref="ScoreSaberSong"/>.
        /// </summary>
        public ScoreSaberSong(string hash)
            : base(hash)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ScoreSaberSong"/>.
        /// </summary>
        public ScoreSaberSong(string hash, string? songName, string? mapperName)
            : base(hash, songName, mapperName)
        {
        }


        /// <summary>
        /// Creates a new <see cref="ScoreSaberSong"/>.
        /// </summary>
        public ScoreSaberSong(string hash, string? songName, string? mapperName,
            Uri? downloadUri, Uri? sourceUri)
            : base(hash, songName, mapperName, downloadUri, sourceUri)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ScoreSaberSong"/>.
        /// </summary>
        public ScoreSaberSong(string hash, string? songName, string? mapperName,
            Uri? downloadUri, Uri? sourceUri, JObject? jsonData)
            : base(hash, songName, mapperName, downloadUri, sourceUri, jsonData)
        {
        }

        /// <inheritdoc/>
        public override void UpdateFrom(ScrapedSong other, bool updateJson = true)
        {
            if (other is ScoreSaberSong song && (song.Ranked || !Ranked))
            {
                ScoreSaberId = song.ScoreSaberId;
                SongAuthorName = song.SongAuthorName;
                BPM = song.BPM;
                Difficulty = song.Difficulty;
                Scores = song.Scores;
                Ranked = song.Ranked;
                Stars = song.Stars;
            }
            else
            {
                base.UpdateFrom(other, false);
            }
        }

        /// <summary>
        /// ScoreSaber UID for the difficulty.
        /// </summary>
        [JsonProperty("uid")]
        public int ScoreSaberId { get; set; }
        /// <summary>
        /// Song artist.
        /// </summary>
        [JsonProperty("songAuthorName")]
        public string? SongAuthorName { get; set; }
        /// <summary>
        /// Beats Per Minute.
        /// </summary>
        [JsonProperty("bpm")]
        public int BPM { get; set; }
        /// <summary>
        /// Difficulty and characteristic (ScoreSaber).
        /// </summary>
        [JsonProperty("diff")]
        public string? Difficulty { get; set; }
        /// <summary>
        /// Number of scores set.
        /// </summary>
        public int Scores { get; set; }
        /// <summary>
        /// Scores set today?
        /// </summary>
        [JsonProperty("scores_day")]
        public int ScoresPerDay { get; set; }
        /// <summary>
        /// True if the song (difficulty) is ranked.
        /// </summary>
        [JsonProperty("ranked")]
        public bool Ranked { get; set; }
        /// <summary>
        /// ScoreSaber star rating.
        /// </summary>
        [JsonProperty("stars")]
        public float Stars { get; set; }

        [JsonProperty("scores")]
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        private string scores_str
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        {
            get => Scores.ToString();
            set
            {
                if (int.TryParse(value,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.NumberFormatInfo.CurrentInfo,
                    out int scores))
                    Scores = scores;
            }
        }
    }
}
