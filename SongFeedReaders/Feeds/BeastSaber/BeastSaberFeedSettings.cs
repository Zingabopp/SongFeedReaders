using System;

namespace SongFeedReaders.Feeds.BeastSaber
{
    /// <summary>
    /// Base class for Beast Saber feed settings
    /// </summary>
    public abstract class BeastSaberFeedSettings : FeedSettingsBase, IPagedFeedSettings
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
    }
}
