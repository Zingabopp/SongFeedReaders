using SongFeedReaders.Attributes;
using SongFeedReaders.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// Base class for a basic <see cref="IFeedFactory"/>.
    /// </summary>
    public abstract class FeedFactoryBase : IFeedFactory
    {
        private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers all <see cref="IFeed"/>s tagged with <see cref="FeedAttribute"/>
        /// from the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int AddAttributedFeeds(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            int feedsAdded = 0;

            foreach (Type? type in assembly.GetTypes()
                .Where(t => typeof(IFeed).IsAssignableFrom(t) && t.GetCustomAttribute<FeedAttribute>() != null))
            {
                FeedAttribute? att = type.GetCustomAttribute<FeedAttribute>();
                if (att != null)
                {
                    Type settingsType = att.SettingsType;
                    RegisterSettingsType(settingsType, type);
                    feedsAdded++;
                }
            }
            return feedsAdded;
        }

        /// <summary>
        /// Registers an <see cref="IFeedSettings"/> type for the given <see cref="IFeed"/> type.
        /// </summary>
        /// <param name="settingsType"></param>
        /// <param name="feedType"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void RegisterSettingsType(Type settingsType, Type feedType)
        {
            if (!typeof(IFeedSettings).IsAssignableFrom(settingsType))
                throw new ArgumentException("settingsType must implement IFeedSettings", nameof(settingsType));
            if (!typeof(IFeed).IsAssignableFrom(feedType))
                throw new ArgumentException("feedType must implement IFeed", nameof(feedType));

            _map[settingsType] = feedType;
        }

        /// <summary>
        /// Attempts to instantiate an <see cref="IFeed"/> of the given <paramref name="feedType"/>.
        /// <see cref="IFeedSettings"/> should not be assigned in most cases.
        /// Return null if the feed type could not be instantiated.
        /// </summary>
        /// <param name="feedType"></param>
        /// <returns></returns>
        protected abstract IFeed? InstantiateFeed(Type feedType);

        /// <inheritdoc/>
        public virtual IFeed GetFeed(IFeedSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (_map.TryGetValue(settings.GetType(), out Type? feedType))
            {
                if (InstantiateFeed(feedType) is IFeed feed)
                {
                    feed.TryAssignSettings(settings);
                    return feed;
                }
                throw new FeedFactoryException($"Feed type '{feedType.Name}' is not a registered service.");
            }
            throw new UnregisteredSettingsTypeException($"Settings type '{settings.GetType().Name}' is not registered.");
        }

        /// <inheritdoc/>
        public virtual async Task<IFeed> GetInitializedFeedAsync(IFeedSettings settings, CancellationToken cancellationToken)
        {
            IFeed feed = GetFeed(settings);
            await feed.InitializeAsync(cancellationToken);
            return feed;
        }

        /// <inheritdoc/>
        public bool SettingsTypeRegistered<TSettings>() where TSettings : IFeedSettings
        {
            return _map.ContainsKey(typeof(TSettings));
        }
        /// <inheritdoc/>
        public bool SettingsTypeRegistered(Type settingsType)
        {

            return _map.ContainsKey(settingsType);
        }
    }
}
