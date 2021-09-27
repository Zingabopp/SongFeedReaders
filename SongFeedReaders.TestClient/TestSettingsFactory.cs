using SongFeedReaders.Feeds;
using SongFeedReaders.Services;
using System.Collections.Concurrent;
using System.Linq;

namespace SongFeedReaders.TestClient
{
    public class TestSettingsFactory : ISettingsFactory
    {
        private readonly ConcurrentDictionary<string, IFeedSettings> settingsMap = new ConcurrentDictionary<string, IFeedSettings>();

        public void RegisterSetting<TSettings>()
            where TSettings : class, IFeedSettings, new()
        {
            TSettings settings = new TSettings();
            settingsMap[settings.FeedId] = settings;
        }
        public void RegisterSetting<TSettings>(TSettings settings)
            where TSettings : class, IFeedSettings
        {
            settingsMap[settings.FeedId] = settings;
        }

        public IFeedSettings? GetSettings(string feedId)
        {
            if (settingsMap.TryGetValue(feedId, out IFeedSettings? settings))
                return settings;
            return null;
        }

        public TSettings? RemoveSettings<TSettings>(string? feedId = null)
            where TSettings : class, IFeedSettings
        {
            if (feedId == null)
            {
                IFeedSettings[]? settings = settingsMap.Values.ToArray();
                TSettings? match = settings.FirstOrDefault(s => s is TSettings && s.FeedId == feedId) as TSettings;
                if (match != null)
                {
                    settingsMap.TryRemove(match.FeedId, out _);
                    return match;
                }
            }
            if (feedId != null)
                if (settingsMap.TryRemove(feedId, out IFeedSettings? settings))
                    return settings as TSettings;
            return null;
        }
    }
}
