using SongFeedReaders.Feeds;
using System;
using System.Runtime.Serialization;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// A factory to generate <see cref="IFeed"/> for a given <see cref="IFeedSettings"/>.
    /// </summary>
    public interface IFeedFactory
    {
        /*
        /// <summary>
        /// TODO: Don't use this? IFeed should be constructed with the settings.
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <typeparam name="TFeed"></typeparam>
        void Register<TSettings, TFeed>()
            where TSettings : IFeedSettings
            where TFeed : class, IFeed, new();
        */

        /// <summary>
        /// Registers a factory to generate a feed for a given <see cref="IFeedSettings"/> type.
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <param name="factory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void Register<TSettings>(Func<IFeedSettings, IFeed> factory);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsType"></param>
        /// <param name="factory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        void Register(Type settingsType, Func<IFeedSettings, IFeed> factory);

        /// <summary>
        /// Gets a new feed for the given <see cref="IFeedSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnregisteredSettingsTypeException"></exception>
        /// <exception cref="FeedFactoryExecuteException"></exception>
        IFeed GetFeed(IFeedSettings settings);
        /// <summary>
        /// Returns true if the specified <see cref="IFeedSettings"/> is registered.
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <returns></returns>
        bool SettingsTypeRegistered<TSettings>()
            where TSettings : IFeedSettings;
        /// <summary>
        /// Returns true if the given <see cref="IFeedSettings"/> is registered.
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool SettingsTypeRegistered<TSettings>(TSettings settings)
            where TSettings : IFeedSettings;
    }

    /// <summary>
    /// Base <see cref="Exception"/> for errors thrown by <see cref="IFeedFactory"/>.
    /// </summary>
    public class FeedFactoryException : Exception
    {
        /// <inheritdoc/>
        public FeedFactoryException()
        {
        }

        /// <inheritdoc/>
        public FeedFactoryException(string message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public FeedFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc/>
        public FeedFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when <see cref="IFeedFactory.GetFeed(IFeedSettings)"/> is called with
    /// an unregistered <see cref="IFeedSettings"/>.
    /// </summary>
    public class UnregisteredSettingsTypeException : FeedFactoryException
    {
        /// <inheritdoc/>
        public UnregisteredSettingsTypeException()
        {
        }

        /// <inheritdoc/>
        public UnregisteredSettingsTypeException(string message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public UnregisteredSettingsTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc/>
        public UnregisteredSettingsTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This exception is thrown when any <see cref="Exception"/> is thrown when executing a factory.
    /// </summary>
    public class FeedFactoryExecuteException : FeedFactoryException
    {
        /// <inheritdoc/>
        public FeedFactoryExecuteException()
        {
        }

        /// <inheritdoc/>
        public FeedFactoryExecuteException(string message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public FeedFactoryExecuteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc/>
        public FeedFactoryExecuteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
