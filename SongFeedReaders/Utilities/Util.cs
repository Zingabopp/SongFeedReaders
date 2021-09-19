using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Utilities
{
    public static class Util
    {
        private static bool _utcTime = false;
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
