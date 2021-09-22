using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// Stores the results of a feed.
    /// </summary>
    public class FeedResult
    {
        /// <summary>
        /// Dictionary of songs that were read from the feed.
        /// </summary>
        protected readonly Dictionary<string, ScrapedSong> songs;
        /// <summary>
        /// Array of page results.
        /// </summary>
        protected readonly PageReadResult[] pageResults;
        /// <summary>
        /// Returns the songs from the result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScrapedSong> GetSongs()
            => songs.Values.AsEnumerable();
        /// <summary>
        /// Returns the page results.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageReadResult> GetResults()
            => pageResults.AsEnumerable();

        /// <summary>
        /// Number of songs in the result.
        /// </summary>
        public int Count => songs.Count;

        // TODO: More constructors/parameters.
        public FeedResult(IEnumerable<PageReadResult> pageReadResults, IFeedSettings feedSettings)
        {
            songs = new Dictionary<string, ScrapedSong>();
            if (pageReadResults != null)
            {
                pageResults = pageReadResults.ToArray();
                foreach (var page in pageReadResults)
                {
                    foreach (var song in page.Songs())
                    {
                        string? songHash = song.Hash;
                        if(songHash != null && songHash.Length > 0)
                        {
                            songs[songHash] = song;
                        }
                        if (feedSettings.MaxSongs > 0 && songs.Count >= feedSettings.MaxSongs)
                            break;
                    }
                }
            }
            else
            {
                pageResults = Array.Empty<PageReadResult>();
            }
        }
    }
}
