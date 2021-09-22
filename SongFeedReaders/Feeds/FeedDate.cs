using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Feeds
{
    public struct FeedDate
    {
        public static readonly FeedDate Default = new FeedDate(DateTime.MaxValue, DateDirection.Before);
        public DateTime Date;
        public DateDirection Direction;
        public FeedDate(DateTime dateTime, DateDirection direction)
        {
            Date = dateTime;
            Direction = direction;
        }

        public override bool Equals(object? obj)
        {
            return obj is FeedDate date && Equals(date);
        }

        public bool Equals(FeedDate other)
        {
            return Date == other.Date && Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            int hashCode = -1766348949;
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(FeedDate left, FeedDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FeedDate left, FeedDate right)
        {
            return !(left == right);
        }
    }

    public enum DateDirection
    {
        Before,
        After
    }
}
