using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Models
{
    [ProtoContract(SkipConstructor = true)]
    public class AndruzzProtobufContainer
    {
        [ProtoMember(1)]
        public readonly byte formatVersion;
        [ProtoMember(2)]
        public readonly uint scrapeEndedTimeUnix;
        [ProtoMember(4)]
        public readonly AndruzzProtobufSong[] songs = null!;

        public DateTime ScrapeTime => AndruzzProtobufSong.epoch.AddSeconds(scrapeEndedTimeUnix);
        public override string ToString()
        {
            return $"v{formatVersion}|{ScrapeTime:g}";
        }
    }

    [ProtoContract]
    public class AndruzzProtobufSong : ScrapedSong
    {
        internal static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        public enum RankedStatus : uint { Unranked, Ranked = 1, Qualified = 2 }

        [ProtoMember(1)]
        public readonly float bpm;
        [ProtoMember(2)]
        public readonly uint downloadCount;
        [ProtoMember(3)]
        public readonly uint upvotes;
        [ProtoMember(4)]
        public readonly uint downvotes;

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
        [ProtoMember(14)]
        public readonly uint rankedChangeUnix;

        private uint _mapId;
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

        [ProtoMember(8)]
        public readonly uint songDurationSeconds;

        private byte[] _hashBytes = null!;
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


        [ProtoMember(10)]
        private string? songName
        {
            get => Name;
            set => Name = value;
        }

        [ProtoMember(11)]
        public readonly string songAuthorName = null!;
        [ProtoMember(12)]
        public string? levelAuthorName
        {
            get => LevelAuthorName;
            set => LevelAuthorName = value;
        }

        [ProtoMember(15)]
        public readonly RankedStatus rankedState;

        //[ProtoMember(13, OverwriteList = true)]
        //internal readonly AndruzzProtobufDiff[] difficulties = null!;

        AndruzzProtobufSong()
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
