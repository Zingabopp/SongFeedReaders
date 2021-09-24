using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Feeds.BeastSaber;
using SongFeedReaders.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;
using WebUtilities.HttpClientWrapper;
using SongFeedReaders.Feeds.ScoreSaber;

namespace SongFeedReadersTests
{
    [TestClass]
    public class UnitTest1
    {
        public static readonly ILogFactory? LogFactory = null;

        [TestMethod]
        public void ScoreSaberFeed_CreateUri()
        {
            var feedNumber = ScoreSaberFeed.FeedNumber.LatestRanked;
            int songsPerPage = 50;
            int pageNum = 1;
            bool rankedOnly = true;

            Uri uri = ScoreSaberFeed.CreateUri(feedNumber, pageNum, rankedOnly, songsPerPage);
            string str = uri.ToString();
        }


        [TestMethod]
        public async Task TestScoreSaberLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            ScoreSaberPageHandler pageHandler = new ScoreSaberPageHandler();
            ScoreSaberLatestSettings feedSettings = new ScoreSaberLatestSettings()
            {
                MaxSongs = 57,
                RankedOnly = true
            };
            IFeed feed = new ScoreSaberLatestFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }

        [TestMethod]
        public async Task TestBeatSaverLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler();
            BeatSaverLatestSettings feedSettings = new BeatSaverLatestSettings()
            {
                 Filter = s =>
                 {
                     return !s.Key?.Contains("2") ?? throw new Exception("Null song key");
                 },
                MaxSongs = 57
            };
            IFeed feed = new BeatSaverLatestFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }

        [TestMethod]
        public async Task TestBeatSaverMapper()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler();
            BeatSaverMapperSettings feedSettings = new BeatSaverMapperSettings()
            {
                Filter = s =>
                {
                    return !s.Key?.Contains("2") ?? throw new Exception("Null song key");
                },
                MaxSongs = 57,
                MapperName = "rustic"
            };
            IFeed feed = new BeatSaverMapperFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }

        [TestMethod]
        public async Task TestBeastSaber_Follows()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberFollowsSettings feedSettings = new BeastSaberFollowsSettings()
            {
                Username = "Zingabopp",
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberFollowsFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
        [TestMethod]
        public async Task TestBeastSaber_Curator()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberCuratorSettings feedSettings = new BeastSaberCuratorSettings()
            {
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberCuratorFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
        [TestMethod]
        public async Task TestBeastSaber_Bookmarks()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberBookmarksSettings feedSettings = new BeastSaberBookmarksSettings()
            {
                Username = "Zingabopp",
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberBookmarksFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
    }
}
