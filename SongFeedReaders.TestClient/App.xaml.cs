using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeastSaber;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Feeds.ScoreSaber;
using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using SongFeedReaders.TestClient.ViewModels;
using SongFeedReaders.TestClient.Views;
using WebUtilities;
using WebUtilities.HttpClientWrapper;

namespace SongFeedReaders.TestClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal ServiceProvider serviceProvider = null!;
        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);
            ServiceProviderOptions options = new ServiceProviderOptions()
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            };
            serviceProvider = services.BuildServiceProvider(options);
        }

        /// <summary>
        /// Configures dependency graph.
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IWebClient>(new HttpClientWrapper("SongFeedReaders_TestClient/1.0.0"));
            services.AddSingleton<ILogFactory>(new LogFactory(m => new FeedReaderLogger() { ModuleName = m }));
            services.AddSingleton<ISettingsFactory>(BuildSettingsFactory());
            services.AddSingleton<IBeatSaverPageHandler, BeatSaverPageHandler>();
            services.AddSingleton<IBeastSaberPageHandler, BeastSaberPageHandler>();
            services.AddSingleton<IScoreSaberPageHandler, ScoreSaberPageHandler>();
            services.AddTransient<IFeed, BeatSaverLatestFeed>();
            services.AddTransient<IFeed, BeatSaverMapperFeed>();
            services.AddTransient<IFeed, BeastSaberBookmarksFeed>();
            services.AddTransient<IFeed, BeastSaberFollowsFeed>();
            services.AddTransient<IFeed, BeastSaberCuratorFeed>();
            services.AddTransient<IFeed, ScoreSaberLatestFeed>();
            services.AddTransient<IFeed, ScoreSaberTrendingFeed>();
            services.AddTransient<IFeed, ScoreSaberTopPlayedFeed>();
            services.AddTransient<IFeed, ScoreSaberTopRankedFeed>();
            services.AddTransient<NavigationViewModel>();
            services.AddTransient<NavigationView>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetRequiredService<NavigationView>();
            mainWindow.Show();
        }

        private static ISettingsFactory BuildSettingsFactory()
        {
            TestSettingsFactory factory = new TestSettingsFactory();
            factory.RegisterSetting<BeatSaverLatestSettings>();
            factory.RegisterSetting<BeatSaverMapperSettings>(new BeatSaverMapperSettings() { MapperName = "rustic" });
            factory.RegisterSetting<BeastSaberBookmarksSettings>(new BeastSaberBookmarksSettings() { Username = "Zingabopp" });
            factory.RegisterSetting<BeastSaberFollowsSettings>(new BeastSaberFollowsSettings() { Username = "Zingabopp" });
            factory.RegisterSetting<BeastSaberCuratorSettings>();
            var ssLS = new ScoreSaberLatestSettings();
            ssLS.SetSongsPerPage(501);
            factory.RegisterSetting<ScoreSaberLatestSettings>(ssLS);
            factory.RegisterSetting<ScoreSaberTrendingSettings>();
            factory.RegisterSetting<ScoreSaberTopPlayedSettings>();
            factory.RegisterSetting<ScoreSaberTopRankedSettings>();
            return factory;
        }
    }
}
