using SongFeedReaders.Feeds;
using System;
using System.Runtime.Serialization;

namespace SongFeedReaders.Services
{
    public interface IFeedFactory
    {
        void Register<TSettings, TFeed>()
            where TSettings : IFeedSettings
            where TFeed : class, IFeed, new();
        void Register<TSettings, TFeed>(Func<TSettings, TFeed> factory)
            where TSettings : IFeedSettings
            where TFeed : class, IFeed;

        /// <summary>
        /// Gets a new feed for the given <see cref="IFeedSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredSettingsTypeException"></exception>
        IFeed GetFeed(IFeedSettings settings);
    }

    public abstract class FeedFactoryException : Exception
    {
        protected FeedFactoryException()
        {
        }

        protected FeedFactoryException(string message) : base(message)
        {
        }

        protected FeedFactoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected FeedFactoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UnregisteredSettingsTypeException : FeedFactoryException
    {
        public UnregisteredSettingsTypeException()
        {
        }

        public UnregisteredSettingsTypeException(string message)
            : base(message)
        {
        }

        public UnregisteredSettingsTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public UnregisteredSettingsTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
