using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeatSaver;
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
    }
}
