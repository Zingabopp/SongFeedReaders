using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Utilities
{
    /// <summary>
    /// Useful static utilities.
    /// </summary>
    public static class Util
    {
        private const string WebTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'Z'";

        private static bool _utcTime = false;
        /// <summary>
        /// Returns the current <see cref="DateTime"/>. Uses <see cref="DateTime.UtcNow"/>
        /// if <see cref="DateTime.Now"/> fails (can happen on certain Mono runtimes with certain locales apparently).
        /// </summary>
        public static DateTime Now
        {
            get
            {
                if(!_utcTime)
                {
                    try
                    {
                        return DateTime.Now;
                    }
                    catch (Exception)
                    {
                        _utcTime = true;
                    }
                }
                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Converts the given <see cref="DateTime"/> to UTC web format.
        /// Used for Beat Saver's API.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToUTCWebTime(this DateTime dateTime)
            => dateTime.ToUniversalTime().ToString(WebTimeFormat);
    }
}
