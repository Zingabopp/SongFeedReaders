using SongFeedReaders.Feeds;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// A factory to generate <see cref="IFeed"/> for a given <see cref="IFeedSettings"/>.
    /// </summary>
    public interface IFeedFactory
    {
        /// <summary>
        /// Gets a new uninitialized feed for the given <see cref="IFeedSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnregisteredSettingsTypeException"></exception>
        /// <exception cref="FeedFactoryExecuteException"></exception>
        /// <exception cref="FeedFactoryException"></exception>
        IFeed GetFeed(IFeedSettings settings);
        /// <summary>
        /// Returns an <see cref="IFeed"/> that has been initialized with the given <see cref="IFeedSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnregisteredSettingsTypeException"></exception>
        /// <exception cref="FeedFactoryExecuteException"></exception>
        /// <exception cref="FeedFactoryException"></exception>
        /// <exception cref="FeedInitializationException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<IFeed> GetInitializedFeedAsync(IFeedSettings settings, CancellationToken cancellationToken);
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
    /// This exception is thrown when any <see cref="Exception"/> is thrown when constructing a feed.
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
