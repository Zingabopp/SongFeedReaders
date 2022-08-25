using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Services.SongInfoProviders;
using System.Threading.Tasks;
using WebUtilities.HttpClientWrapper;

namespace SongFeedReadersTests.SongInfoProviders
{
    [TestClass]
    public class AndruzzProviderTests
    {
        [TestMethod]
        public async Task Initialize()
        {
            string expectedHash = "19F2879D11A91B51A5C090D63471C3E8D9B7AEE3";
            string expectedName = "Believer";
            HttpClientWrapper webClient = new HttpClientWrapper();
            AndruzzScrapedInfoProvider provider = new AndruzzScrapedInfoProvider(webClient, null);
            SongFeedReaders.Models.ScrapedSong? songInfo = await provider.GetSongByKeyAsync("b");
            Assert.IsNotNull(songInfo);
            Assert.AreEqual(expectedHash, songInfo.Hash);
            Assert.AreEqual(expectedName, songInfo.Name);
        }
    }
}
