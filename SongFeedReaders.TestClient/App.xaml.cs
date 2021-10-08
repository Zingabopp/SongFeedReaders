using Microsoft.Extensions.DependencyInjection;
using SongFeedReaders.Attributes;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeastSaber;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Feeds.ScoreSaber;
using SongFeedReaders.Logging;
using SongFeedReaders.Services;
using SongFeedReaders.TestClient.ViewModels;
using SongFeedReaders.TestClient.Views;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
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
            services.AddSingleton<IBeatSaverPageHandler, BeatSaverPageHandler>();
            services.AddSingleton<IBeastSaberPageHandler, BeastSaberPageHandler>();
            services.AddSingleton<IScoreSaberPageHandler, ScoreSaberPageHandler>();
            ConfigureFeeds(services);
            ConfigureSettings(services);
            services.AddSingleton<IFeedFactory, TestFeedFactory>();
            services.AddTransient<NavigationViewModel>();
            services.AddTransient<NavigationView>();
        }
        /// <summary>
        /// Registers all feeds with the <see cref="FeedAttribute"/>.
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureFeeds(ServiceCollection services)
        {
            foreach (Type? type in typeof(IFeed).Assembly.GetTypes().Where(t => t.GetCustomAttribute<FeedAttribute>() != null))
            {
                FeedAttribute? att = type.GetCustomAttribute<FeedAttribute>();
                if (att != null)
                {
                    services.AddTransient(type);
                }
            }
        }
        /// <summary>
        /// Registers a collection of feed settings.
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureSettings(ServiceCollection services)
        {
            services.AddSingleton<IFeedSettings>(new BeatSaverLatestSettings());
            services.AddSingleton<IFeedSettings>(new BeatSaverMapperSettings() { MapperName = "rustic" });
            services.AddSingleton<IFeedSettings>(new BeatSaverMapperSettings() { MapperName = "ruckus" });
            services.AddSingleton<IFeedSettings>(new BeastSaberBookmarksSettings() { Username = "Zingabopp" });
            services.AddSingleton<IFeedSettings>(new BeastSaberFollowsSettings() { Username = "Zingabopp" });
            services.AddSingleton<IFeedSettings>(new BeastSaberCuratorSettings());
            ScoreSaberLatestSettings? ssLS = new ScoreSaberLatestSettings();
            ssLS.SetSongsPerPage(501);
            services.AddSingleton<IFeedSettings>(ssLS);
            services.AddSingleton<IFeedSettings>(new ScoreSaberTrendingSettings());
            services.AddSingleton<IFeedSettings>(new ScoreSaberTopPlayedSettings());
            services.AddSingleton<IFeedSettings>(new ScoreSaberTopRankedSettings());
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            NavigationView? mainWindow = serviceProvider.GetRequiredService<NavigationView>();
            mainWindow.Show();
        }
    }
}
