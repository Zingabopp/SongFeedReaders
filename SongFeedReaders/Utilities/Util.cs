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
    }
}
