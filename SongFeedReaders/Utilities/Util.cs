using SongFeedReaders.Logging;
using System;
using System.Linq;

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
                if (!_utcTime)
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


        /// <summary>
        /// Raises an event, catching any exceptions that occur, optionally logging any exceptions.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        /// <param name="logger"></param>
        public static void RaiseEventSafe(this EventHandler? e, object sender,
            string eventName, ILogger? logger = null)
        {
            if (e == null) return;
            EventHandler[] handlers = e.GetInvocationList().Select(d => (EventHandler)d).ToArray()
                ?? Array.Empty<EventHandler>();
            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    handlers[i].Invoke(sender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    logger?.Error($"Error in '{eventName}' handlers '{handlers[i]?.Method.Name}': {ex.Message}");
                    logger?.Debug(ex);
                }
            }
        }
        /// <summary>
        /// Raises an event, catching any exceptions that occur, optionally logging any exceptions.
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="eventName"></param>
        /// <param name="logger"></param>
        public static void RaiseEventSafe<TArgs>(this EventHandler<TArgs>? e, object sender, TArgs args,
            string eventName, ILogger? logger = null)
        {
            if (e == null) return;
            EventHandler<TArgs>[] handlers = e.GetInvocationList().Select(d => (EventHandler<TArgs>)d).ToArray()
                ?? Array.Empty<EventHandler<TArgs>>();
            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    handlers[i].Invoke(sender, args);
                }
                catch (Exception ex)
                {
                    logger?.Error($"Error in '{eventName}' handlers '{handlers[i]?.Method.Name}': {ex.Message}");
                    logger?.Debug(ex);
                }
            }
        }
    }
}
