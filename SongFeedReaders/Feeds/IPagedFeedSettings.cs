namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Settings for a feed that lists songs by page.
    /// </summary>
    public interface IPagedFeedSettings : IFeedSettings
    {
        /// <summary>
        /// Page of the feed to start on, default is 1. 
        /// For all feeds, setting '1' here is the same as starting on the first page.
        /// </summary>
        int StartingPage { get; set; }
    }
}
