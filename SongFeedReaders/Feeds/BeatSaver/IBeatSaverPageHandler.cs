using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
