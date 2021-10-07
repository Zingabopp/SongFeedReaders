using System;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Defines a <see cref="DateTime"/> and <see cref="DateDirection"/> to move in a feed.
    /// </summary>
    public struct FeedDate
    {
        /// <summary>
        /// Default value starting from latest and moving back.
        /// </summary>
        public static readonly FeedDate Default = new FeedDate(DateTime.MaxValue, DateDirection.Before);
        /// <summary>
        /// DateTime to move from in the feed.
        /// </summary>
        public DateTime Date;
        /// <summary>
        /// Direction to move in the feed.
        /// </summary>
        public DateDirection Direction;
        /// <summary>
        /// Creates a new <see cref="FeedDate"/>.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="direction"></param>
        public FeedDate(DateTime dateTime, DateDirection direction)
        {
            Date = dateTime;
            Direction = direction;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is FeedDate date && Equals(date);
        }
        /// <summary>
        /// Returns true if if <paramref name="other"/> is the same as this instance.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FeedDate other)
        {
            return Date == other.Date && Direction == other.Direction;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1766348949;
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(FeedDate left, FeedDate right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(FeedDate left, FeedDate right)
        {
            return !(left == right);
        }
    }
    /// <summary>
    /// Direction to move from a date.
    /// </summary>
    public enum DateDirection
    {
        /// <summary>
        /// Move back in time.
        /// </summary>
        Before,
        /// <summary>
        /// Move forward in time.
        /// </summary>
        After
    }
}
