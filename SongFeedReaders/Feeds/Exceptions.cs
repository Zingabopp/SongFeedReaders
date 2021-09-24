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
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFeedSettingsException"/> class.
        /// </summary>
        public InvalidFeedSettingsException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFeedSettingsException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public InvalidFeedSettingsException(string message)
            : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFeedSettingsException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidFeedSettingsException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// This exception is thrown when an error occurs while reading a feed.
    /// </summary>
    public class FeedReaderException : Exception
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
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        public FeedReaderException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FeedReaderException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="reason"></param>
        public FeedReaderException(string message, FeedReaderFailureCode reason)
            : base(message)
        {
            FailureCode = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
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
    /// This exception is thrown when an error occurs filtering songs in a feed.
    /// </summary>
    public class FeedFilterException : FeedReaderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FeedFilterException(string message)
            : base(message, FeedReaderFailureCode.FilterFailed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FeedFilterException(string message, Exception? innerException)
            : base(message, innerException, FeedReaderFailureCode.FilterFailed)
        {
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
        Cancelled = 3,
        /// <summary>
        /// Exception thrown during song filtering.
        /// </summary>
        FilterFailed = 4
    }

    /// <summary>
    /// This exception is thrown when there's an error parsing a page's content.
    /// </summary>
    public class PageParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageParseException"/> class.
        /// </summary>
        public PageParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageParseException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public PageParseException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageParseException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PageParseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageParseException"/> class with serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PageParseException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// This exception is thrown when a feed initialization fails.
    /// </summary>
    public class FeedInitializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedInitializationException"/> class.
        /// </summary>
        public FeedInitializationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedInitializationException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FeedInitializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedInitializationException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FeedInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedInitializationException"/> class with serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FeedInitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// This exception is thrown when attempting to use an uninitialized feed.
    /// </summary>
    public class FeedUninitializedException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedUninitializedException"/> class.
        /// </summary>
        public FeedUninitializedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedUninitializedException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FeedUninitializedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedUninitializedException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FeedUninitializedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedUninitializedException"/> class with serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FeedUninitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
