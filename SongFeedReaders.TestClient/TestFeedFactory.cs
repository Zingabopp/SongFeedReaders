using SongFeedReaders.Attributes;
using SongFeedReaders.Feeds;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.TestClient
{
    internal class TestFeedFactory : IFeedFactory
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        public TestFeedFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            foreach (var type in typeof(IFeed).Assembly.GetTypes().Where(t => t.GetCustomAttribute<FeedAttribute>() != null))
            {
                FeedAttribute? att = type.GetCustomAttribute<FeedAttribute>();
                if (att != null)
                {
                    Type settingsType = att.SettingsType;
                    RegisterSettingsType(settingsType, type);
                }
            }
        }

        public void RegisterSettingsType<TSettingsType, TFeedType>()
            where TSettingsType : IFeedSettings
            where TFeedType : IFeed, new()
        {
            _map[typeof(TSettingsType)] = typeof(TFeedType);
        }

        public void RegisterSettingsType(Type settingsType, Type feedType)
        {
            if (!settingsType.IsAssignableTo(typeof(IFeedSettings)))
                throw new ArgumentException("settingsType must implement IFeedSettings", nameof(settingsType));
            if(!feedType.IsAssignableTo(typeof(IFeed)))
                throw new ArgumentException("feedType must implement IFeed", nameof(feedType));

            _map[settingsType] = feedType;
        }

        public IFeed GetFeed(IFeedSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if(_map.TryGetValue(settings.GetType(), out Type? feedType))
            {
                if(_serviceProvider.GetService(feedType) is IFeed feed)
                {
                    feed.TryAssignSettings(settings);
                    return feed;
                }
                throw new FeedFactoryException($"Feed type '{feedType.Name}' is not a registered service.");
            }
            throw new UnregisteredSettingsTypeException($"Settings type '{settings.GetType().Name}' is not registered.");
        }

        public async Task<IFeed> GetInitializedFeedAsync(IFeedSettings settings, CancellationToken cancellationToken)
        {
            IFeed feed = GetFeed(settings);
            await feed.InitializeAsync(cancellationToken);
            return feed;
        }

        public bool SettingsTypeRegistered<TSettings>() where TSettings : IFeedSettings
        {
            return _map.ContainsKey(typeof(TSettings));
        }

        public bool SettingsTypeRegistered<TSettings>(TSettings settings) where TSettings : IFeedSettings
        {
            return _map.ContainsKey(typeof(TSettings));
        }
    }
}
