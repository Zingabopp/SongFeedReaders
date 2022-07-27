using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeastSaber;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Feeds.ScoreSaber;
using SongFeedReaders.Logging;
using SongFeedReaders.Utilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebUtilities;
using WebUtilities.HttpClientWrapper;

namespace SongFeedReadersTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ScoreSaberFeed_CreateUri()
        {
            FeedNumber feedNumber = FeedNumber.LatestRanked;
            int songsPerPage = 50;
            int pageNum = 1;
            bool rankedOnly = true;
            string expectedUrl = "https://scoresaber.com/api.php?function=get-leaderboards" +
                $"&cat={(int)feedNumber}&limit={songsPerPage}&page={pageNum}&ranked={(rankedOnly ? 1 : 0)}";

            Uri uri = ScoreSaberFeed<ScoreSaberFeedSettings>.CreateUri(feedNumber, pageNum, rankedOnly, songsPerPage);
            Assert.AreEqual(expectedUrl, uri.ToString());
        }


        [TestMethod]
        public async Task TestScoreSaberLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            int maxSongs = 57;
            ScoreSaberPageHandler pageHandler = new ScoreSaberPageHandler();
            ScoreSaberLatestSettings feedSettings = new ScoreSaberLatestSettings()
            {
                MaxSongs = maxSongs,
                RankedOnly = true
            };
            ScoreSaberLatestFeed? feed = new ScoreSaberLatestFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }

        [TestMethod]
        public async Task TestBeatSaverLatest()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
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
            BeatSaverLatestFeed? feed = new BeatSaverLatestFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }

        [TestMethod]
        public async Task TestBeatSaverCurated()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            int maxSongs = 57;
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler();
            BeatSaverCuratorSettings feedSettings = new BeatSaverCuratorSettings()
            {
                MaxSongs = maxSongs
            };
            BeatSaverCuratorFeed? feed = new BeatSaverCuratorFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
            
        }

        [TestMethod]
        public async Task TestBeatSaverMapper()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
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
            BeatSaverMapperFeed? feed = new BeatSaverMapperFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.IsTrue(songs.Length <= maxSongs);
        }

        [TestMethod]
        public async Task TestBeastSaber_Follows()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberFollowsSettings feedSettings = new BeastSaberFollowsSettings()
            {
                Username = "Zingabopp",
                MaxSongs = maxSongs
            };
            BeastSaberFollowsFeed? feed = new BeastSaberFollowsFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
        [TestMethod]
        public async Task TestBeastSaber_Curator()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberCuratorSettings feedSettings = new BeastSaberCuratorSettings()
            {
                MaxSongs = maxSongs
            };
            BeastSaberCuratorFeed? feed = new BeastSaberCuratorFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
        [TestMethod]
        public async Task TestBeastSaber_Bookmarks()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            int maxSongs = 57;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberBookmarksSettings feedSettings = new BeastSaberBookmarksSettings()
            {
                Username = "Zingabopp",
                MaxSongs = maxSongs
            };
            BeastSaberBookmarksFeed? feed = new BeastSaberBookmarksFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            FeedResult? result = await feed.ReadAsync(PauseToken.None, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }


        [TestMethod]
        public async Task TestPause_BeastSaber_Curator()
        {
            ILogFactory logFactory = Utilities.DefaultLogFactory;
            IWebClient client = MockClientSetup.GetMockClient();
            var logger = logFactory.GetLogger(GetType().Name);
            PauseTokenSource pts = new PauseTokenSource(logger);
            CancellationTokenSource cts = new CancellationTokenSource();
            pts.Paused += (s, e) =>
            {
                logger.Info($"----Paused-----");
            };
            pts.Resumed += (s, e) =>
            {
                logger.Info($"----Resumed----");
            };
            int maxSongs = 100;
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberCuratorSettings feedSettings = new BeastSaberCuratorSettings()
            {
                MaxSongs = maxSongs
            };
            BeastSaberCuratorFeed? feed = new BeastSaberCuratorFeed(pageHandler, client, logFactory);
            feed.TryAssignSettings(feedSettings);
            await feed.InitializeAsync(CancellationToken.None);
            IProgress<PageReadResult> progress = new Progress<PageReadResult>(async r =>
            {
                logger.Info($"{r.SongsOnPage} songs read on page '{r.Uri}'");
                using var reg = pts.Pause();
                logger.Info($"Pausing for 2s");
                await Task.Delay(2000);
                logger.Info($"Finished pausing");
            });
            FeedResult? result = await feed.ReadAsync(progress, pts.Token, cts.Token).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            PageReadResult[]? pages = result.GetResults().ToArray();
            SongFeedReaders.Models.ScrapedSong[]? songs = result.GetSongs().ToArray();
            Assert.IsTrue(pages.Length > 0);
            Assert.AreEqual(maxSongs, songs.Length);
        }
    }
}
