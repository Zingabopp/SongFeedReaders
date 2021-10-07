using SongFeedReaders.Models;
using System;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Handles reading content from a Beat Saver page.
    /// </summary>
    public interface IBeatSaverPageHandler : IFeedPageHandler
    {
        /// <summary>
        /// Parses a Beat Saver user ID from their user detail page.
        /// Should return an empty string if the ID was not found and no other errors occurred.
        /// <see href="https://api.beatsaver.com/users/name/"/>
        /// </summary>
        /// <param name="pageText"></param>
        /// <returns></returns>
        /// <exception cref="PageParseException"></exception>
        string ParseUserIdFromPage(string pageText);

        /// <summary>
        /// Parses a single song from a Beat Saver song details page.
        /// </summary>
        /// <param name="pageText"></param>
        /// <param name="sourceUri"></param>
        /// <param name="storeRawData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PageParseException"></exception>
        ScrapedSong ParseSingle(string pageText, Uri? sourceUri, bool storeRawData);
    }
}
