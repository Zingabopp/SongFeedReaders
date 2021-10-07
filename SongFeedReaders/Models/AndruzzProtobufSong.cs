using ProtoBuf;
using System;

namespace SongFeedReaders.Models
{
    /// <summary>
    /// Data class to deserialize an Andruzz Scrapped Data protobuf container.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class AndruzzProtobufContainer
    {
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0051 // Remove unused private members
        /// <summary>
        /// Format version.
        /// </summary>
        [ProtoMember(1)]
        public readonly byte formatVersion;
        /// <summary>
        /// Unix time the scrape finished.
        /// </summary>
        [ProtoMember(2)]
        public readonly uint scrapeEndedTimeUnix;
        /// <summary>
        /// Array of songs.
        /// </summary>
        [ProtoMember(4)]
        public readonly AndruzzProtobufSong[] songs = null!;
        /// <summary>
        /// <see cref="DateTime"/> the scrape finished.
        /// </summary>
        public DateTime ScrapeTime => AndruzzProtobufSong.epoch.AddSeconds(scrapeEndedTimeUnix);
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"v{formatVersion}|{ScrapeTime:g}";
        }
    }

    /// <summary>
    /// Data class to deserialize an Andruzz Scrapped Data protobuf song into.
    /// </summary>
    [ProtoContract]
    public class AndruzzProtobufSong : ScrapedSong
    {
        internal static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

        /// <summary>
        /// Ranked status of a song
        /// </summary>
        public enum RankedStatus : uint
        {
            /// <summary>
            /// Song is not ranked.
            /// </summary>
            Unranked,
            /// <summary>
            /// Song is ranked.
            /// </summary>
            Ranked = 1,
            /// <summary>
            /// Song is qualified.
            /// </summary>
            Qualified = 2
        }
        /// <summary>
        /// Beats per minute.
        /// </summary>
        [ProtoMember(1)]
        public readonly float bpm;
        /// <summary>
        /// Number of downloads.
        /// </summary>
        [ProtoMember(2)]
        public readonly uint downloadCount;
        /// <summary>
        /// Number of upvotes.
        /// </summary>
        [ProtoMember(3)]
        public readonly uint upvotes;
        /// <summary>
        /// Number of downvotes.
        /// </summary>
        [ProtoMember(4)]
        public readonly uint downvotes;

        /// <summary>
        /// Upload Unix time.
        /// </summary>
        [ProtoMember(5)]
        public uint uploadTimeUnix
        {
            get
            {
                return (uint)(UploadDate - epoch).TotalSeconds;
            }
            set
            {
                UploadDate = epoch.AddSeconds(value);
            }
        }
        /// <summary>
        /// Last ranked status change in Unix time.
        /// </summary>
        [ProtoMember(14)]
        public readonly uint rankedChangeUnix;

        private uint _mapId;
        /// <summary>
        /// BeatSaver beatmap ID.
        /// </summary>
        [ProtoMember(6)]
        public uint mapId
        {
            get
            {
                return _mapId;
            }
            set
            {
                _mapId = value;
                Key = value.ToString("X");
            }
        }

        /// <summary>
        /// Song duration in seconds.
        /// </summary>
        [ProtoMember(8)]
        public readonly uint songDurationSeconds;

        private byte[] _hashBytes = null!;
        /// <summary>
        /// Beatmap hash as a byte array.
        /// </summary>
        [ProtoMember(9, OverwriteList = true)]
        public byte[] hashBytes
        {
            get
            {
                return _hashBytes;
            }
            set
            {
                _hashBytes = value;
                Hash = BitConverter.ToString(value).Replace("-", ""); ;
            }
        }

        /// <summary>
        /// Song name.
        /// </summary>
        [ProtoMember(10)]
        private string? songName
        {
            get => Name;
            set => Name = value;
        }
        /// <summary>
        /// Song author name.
        /// </summary>
        [ProtoMember(11)]
        public readonly string songAuthorName = null!;
        /// <summary>
        /// Level author name.
        /// </summary>
        [ProtoMember(12)]
        public string? levelAuthorName
        {
            get => LevelAuthorName;
            set => LevelAuthorName = value;
        }
        /// <summary>
        /// Ranked status.
        /// </summary>
        [ProtoMember(15)]
        public readonly RankedStatus rankedState;

        //[ProtoMember(13, OverwriteList = true)]
        //internal readonly AndruzzProtobufDiff[] difficulties = null!;

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE1006 // Naming Styles
        private AndruzzProtobufSong()
        {
            songDurationSeconds = 1;
            rankedState = RankedStatus.Unranked;
        }
    }
    // Unneeded for now.
    /*
    [ProtoContract]
    public class AndruzzProtobufDiff
    {
        public enum MapDifficulty : byte { Easy = 0, Normal, Hard, Expert, ExpertPlus }
        public enum MapCharacteristic : byte { Custom = 0, Standard, OneSaber, NoArrows, NinetyDegree, ThreeSixtyDegree, Lightshow, Lawless }

        [Flags]
        public enum MapMods : uint { NoodleExtensions = 1, MappingExtensions = 1 << 1, Chroma = 1 << 2, Cinema = 1 << 3 }
#pragma warning disable 649
        [ProtoMember(1)] 
        public readonly MapCharacteristic characteristic = MapCharacteristic.Standard;
        [ProtoMember(2)] 
        public readonly MapDifficulty difficulty;

        [ProtoMember(4)] 
        public readonly uint starsT100;

        [ProtoMember(6)] 
        public readonly uint njsT100;

        [ProtoMember(7)] 
        public readonly uint bombs;
        [ProtoMember(8)] 
        public readonly uint notes;
        [ProtoMember(9)] 
        public readonly uint obstacles;

        [ProtoMember(10)] 
        public readonly MapMods mods;
#pragma warning restore

        AndruzzProtobufDiff()
        {
            difficulty = MapDifficulty.ExpertPlus;
            characteristic = MapCharacteristic.Standard;
        }
    }
    */
}
