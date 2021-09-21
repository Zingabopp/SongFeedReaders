using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SongFeedReaders.Feeds
{
    /// <summary>
    /// This exception is thrown when a method call is invalid for an object's
    /// <see cref="IFeedSettings"/>.
    /// </summary>
    public class InvalidFeedSettingsException : InvalidOperationException
    {
        /// <inheritdoc/>
        public InvalidFeedSettingsException()
            : base() { }

        /// <inheritdoc/>
        public InvalidFeedSettingsException(string message)
            : base(message) { }
        /// <inheritdoc/>
        public InvalidFeedSettingsException(string message, Exception innerException)
            : base(message, innerException) { }
    }

#pragma warning disable CA2237 // Mark ISerializable types with serializable
    /// <summary>
    /// This exception is thrown when an error occurs while reading a feed.
    /// </summary>
    public class FeedReaderException : Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
    {
        /// <summary>
        /// A <see cref="FeedReaderFailureCode"/> associated with the exception.
        /// </summary>
        public FeedReaderFailureCode FailureCode { get; protected set; }
        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(base.Message) && InnerException != null)
                {
                    return InnerException.Message;
                }
                return base.Message;
            }
        }
        /// <inheritdoc />
        public FeedReaderException()
        { }

        /// <inheritdoc />
        public FeedReaderException(string message)
            : base(message)
        { }

        /// <inheritdoc />
        public FeedReaderException(string message, Exception? innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new <see cref="FeedReaderException"/> with the given <see cref="FeedReaderFailureCode"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="reason"></param>
        public FeedReaderException(string message, Exception? innerException, FeedReaderFailureCode reason)
            : base(message, innerException)
        {
            FailureCode = reason;
        }
    }

    /// <summary>
    /// Type of failure that occurred while reading a feed.
    /// </summary>
    public enum FeedReaderFailureCode
    {
        /// <summary>
        /// Generic error.
        /// </summary>
        Generic = 0,
        /// <summary>
        /// All pages failed, likely a site problem.
        /// </summary>
        SourceFailed = 1,
        /// <summary>
        /// Some pages failed.
        /// </summary>
        PageFailed = 2,
        /// <summary>
        /// CancellationToken was triggered before the reader finished.
        /// </summary>
        Cancelled = 3
    }

    /// <summary>
    /// This exception is thrown when there's an error parsing a page's content.
    /// </summary>
    public class PageParseException : Exception
    {
        /// <inheritdoc/>
        public PageParseException()
        {
        }

        /// <inheritdoc/>
        public PageParseException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public PageParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected PageParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
