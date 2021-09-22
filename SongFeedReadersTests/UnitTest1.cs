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

namespace SongFeedReadersTests
{
    [TestClass]
    public class UnitTest1
    {
        public static readonly ILogFactory? LogFactory = null;
        [TestMethod]
        public async Task TestBeatSaver()
        {
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
            IFeed feed = new BeatSaverLatestFeed(feedSettings, pageHandler, client, null);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }

        [TestMethod]
        public async Task TestBeastSaber_Follows()
        {
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberFollowsSettings feedSettings = new BeastSaberFollowsSettings()
            {
                Username = "Zingabopp",
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberFollowsFeed(feedSettings, pageHandler, client, null);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
        [TestMethod]
        public async Task TestBeastSaber_Curator()
        {
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberCuratorSettings feedSettings = new BeastSaberCuratorSettings()
            {
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberCuratorFeed(feedSettings, pageHandler, client, null);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
        [TestMethod]
        public async Task TestBeastSaber_Bookmarks()
        {
            IWebClient client = new HttpClientWrapper();
            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler();
            BeastSaberBookmarksSettings feedSettings = new BeastSaberBookmarksSettings()
            {
                Username = "Zingabopp",
                MaxSongs = 57
            };
            IFeed feed = new BeastSaberBookmarksFeed(feedSettings, pageHandler, client, null);
            var result = await feed.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(result.Count > 0);
            var pages = result.GetResults().ToArray();
            var songs = result.GetSongs().ToArray();
        }
    }
}
