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
        [TestMethod]
        public void ScoreSaberFeed_CreateUri()
        {
            var feedNumber = ScoreSaberFeed.FeedNumber.LatestRanked;
            int songsPerPage = 50;
            int pageNum = 1;
            bool rankedOnly = true;
            string expectedUrl = "https://scoresaber.com/api.php?function=get-leaderboards" +
                $"&cat={(int)feedNumber}&limit={songsPerPage}&page={pageNum}&ranked={(rankedOnly ? 1 : 0)}";

            Uri uri = ScoreSaberFeed.CreateUri(feedNumber, pageNum, rankedOnly, songsPerPage);
            Assert.AreEqual(expectedUrl, uri.ToString());
        }


        [TestMethod]
        public async Task TestScoreSaberLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            ScoreSaberPageHandler pageHandler = new ScoreSaberPageHandler();
            ScoreSaberLatestSettings feedSettings = new ScoreSaberLatestSettings()
            {
                MaxSongs = maxSongs,
                RankedOnly = true
            };
            IFeed feed = new ScoreSaberLatestFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }

        [TestMethod]
        public async Task TestBeatSaverLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler();
            BeatSaverLatestSettings feedSettings = new BeatSaverLatestSettings()
            {
                 Filter = s =>
                 {
                     return !s.Key?.Contains("2") ?? throw new Exception("Null song key");
                 },
                MaxSongs = maxSongs
            };
            IFeed feed = new BeatSaverLatestFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }

        [TestMethod]
        public async Task TestBeatSaverMapper()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler();
            BeatSaverMapperSettings feedSettings = new BeatSaverMapperSettings()
            {
                Filter = s =>
                {
                    return !s.Key?.Contains("2") ?? throw new Exception("Null song key");
                },
                MaxSongs = maxSongs,
                MapperName = "rustic"
            };
            IFeed feed = new BeatSaverMapperFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.IsTrue(songs.Length <= maxSongs);
        }

        [TestMethod]
        public async Task TestBeastSaber_Follows()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberFollowsSettings feedSettings = new BeastSaberFollowsSettings()
            {
                Username = "Zingabopp",
                MaxSongs = maxSongs
            };
            IFeed feed = new BeastSaberFollowsFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
        [TestMethod]
        public async Task TestBeastSaber_Curator()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberCuratorSettings feedSettings = new BeastSaberCuratorSettings()
            {
                MaxSongs = maxSongs
            };
            IFeed feed = new BeastSaberCuratorFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
        [TestMethod]
        public async Task TestBeastSaber_Bookmarks()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = new HttpClientWrapper();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberBookmarksSettings feedSettings = new BeastSaberBookmarksSettings()
            {
                Username = "Zingabopp",
                MaxSongs = maxSongs
            };
            IFeed feed = new BeastSaberBookmarksFeed(feedSettings, pageHandler, client, logFactory);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
    }
}
