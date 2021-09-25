using SongFeedReaders.Models;
using System;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// Holds the response to an <see cref="ISongInfoManager"/> query.
    /// </summary>
    public class SongInfoResponse
    {
        internal static SongInfoResponse FailedResponse = new SongInfoResponse(null, null);
        internal static SongInfoResponse[] EmptyResponseAry = new SongInfoResponse[0];
        /// <summary>
        /// Returns true if the query was successful.
        /// </summary>
        public bool Success { get; }
        /// <summary>
        /// If successful, holds the song information.
        /// </summary>
        public ScrapedSong? Song { get; }
        /// <summary>
        /// The <see cref="ISongInfoProvider"/> that completed this request.
        /// </summary>
        public ISongInfoProvider? Source { get; }
        /// <summary>
        /// Any failed responses that occurred.
        /// </summary>
        internal SongInfoResponse[] FailedResponses;
        /// <summary>
        /// Any exception that was thrown.
        /// </summary>
        public Exception? Exception { get; }
        /// <summary>
        /// Gets an array of any failed responses.
        /// </summary>
        /// <returns></returns>
        public SongInfoResponse[] GetFailedResponses() => FailedResponses;
        /// <summary>
        /// Creates a new <see cref="SongInfoResponse"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="provider"></param>
        public SongInfoResponse(ScrapedSong? song, ISongInfoProvider? provider)
        {
            Song = song;
            Source = provider;
            Success = Song != null;
            FailedResponses = EmptyResponseAry;
        }
        /// <summary>
        /// Creates a new <see cref="SongInfoResponse"/> with an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="provider"></param>
        /// <param name="exception"></param>
        public SongInfoResponse(ScrapedSong? song, ISongInfoProvider? provider, Exception? exception)
            : this(song, provider)
        {
            Exception = exception;
        }
    }

}
