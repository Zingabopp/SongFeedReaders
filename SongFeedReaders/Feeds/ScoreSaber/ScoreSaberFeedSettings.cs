using System;

namespace SongFeedReaders.Feeds.ScoreSaber
{
    /// <summary>
    /// Base class for ScoreSaber feed settings
    /// </summary>
    public abstract class ScoreSaberFeedSettings : FeedSettingsBase, IPagedFeedSettings
    {
        private int _startingPage = 1;
        /// <summary>
        /// Page of the feed to start on, default is 1. Setting '1' here is the same as starting on the first page.
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when set to less than 1.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to less than 1.</exception>
        public int StartingPage
        {
            get { return _startingPage; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(StartingPage), "StartingPage cannot be less than 1.");
                _startingPage = value;
            }
        }
        private int _songsPerPage = 50;

        /// <inheritdoc/>
        public override int SongsPerPage
        {
            get => _songsPerPage;
        }

        /// <summary>
        /// Sets the number of songs per page.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SetSongsPerPage(int value)
        {
            if (value > 0)
                _songsPerPage = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        ///  If true, only return songs with a ranked difficulty.
        /// </summary>
        public bool RankedOnly { get; set; }

        /// <inheritdoc/>
        protected override void CopyTo<T>(T target)
        {
            base.CopyTo(target);
            if (target is ScoreSaberFeedSettings settings)
            {
                settings.RankedOnly = RankedOnly;
                settings.SetSongsPerPage(SongsPerPage);
            }
        }
    }

    /// <summary>
    /// Settings for the <see cref="ScoreSaberTrendingFeed"/>.
    /// </summary>
    public class ScoreSaberTrendingSettings : ScoreSaberFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "ScoreSaber.Trending";

        /// <inheritdoc/>
        public override int FeedIndex => 0;


        /// <inheritdoc/>
        public override object Clone()
        {
            ScoreSaberTrendingSettings? clone = new ScoreSaberTrendingSettings();
            CopyTo(clone);
            return clone;
        }
    }
    /// <summary>
    /// Settings for the <see cref="ScoreSaberTrendingFeed"/>.
    /// </summary>
    public class ScoreSaberLatestSettings : ScoreSaberFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "ScoreSaber.LatestRanked";

        /// <inheritdoc/>
        public override int FeedIndex => 1;


        /// <inheritdoc/>
        public override object Clone()
        {
            ScoreSaberLatestSettings? clone = new ScoreSaberLatestSettings();
            CopyTo(clone);
            return clone;
        }
    }
    /// <summary>
    /// Settings for the <see cref="ScoreSaberTrendingFeed"/>.
    /// </summary>
    public class ScoreSaberTopPlayedSettings : ScoreSaberFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "ScoreSaber.TopPlayed";

        /// <inheritdoc/>
        public override int FeedIndex => 2;


        /// <inheritdoc/>
        public override object Clone()
        {
            ScoreSaberTopPlayedSettings? clone = new ScoreSaberTopPlayedSettings();
            CopyTo(clone);
            return clone;
        }
    }
    /// <summary>
    /// Settings for the <see cref="ScoreSaberTrendingFeed"/>.
    /// </summary>
    public class ScoreSaberTopRankedSettings : ScoreSaberFeedSettings
    {
        /// <inheritdoc/>
        public override string FeedId => "ScoreSaber.TopRanked";

        /// <inheritdoc/>
        public override int FeedIndex => 3;


        /// <inheritdoc/>
        public override object Clone()
        {
            ScoreSaberTopRankedSettings? clone = new ScoreSaberTopRankedSettings();
            CopyTo(clone);
            return clone;
        }
    }
}
