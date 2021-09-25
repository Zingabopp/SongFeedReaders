using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Utilities;
using System;
using System.IO;

namespace SongFeedReadersTests.PageHandler
{
    [TestClass]
    public class BeatSaverPageHandler_Tests
    {
        private readonly string DataPath = Path.GetFullPath(Path.Combine("Data", "BeatSaver"));

        private string GetFilePath(string fileName) => Path.Combine(DataPath, fileName);
        [TestMethod]
        public void StandardPage()
        {
            int expectedSongs = 20;
            int expectedSongsOnPage = 20;
            PageErrorType expectedPageError = PageErrorType.None;
            bool expectedIsLastPage = false;

            Uri uri = new Uri(@"https://api.beatsaver.com/maps/latest");
            string contentType = "application/json";
            string json = File.ReadAllText(GetFilePath("beatsaver_latest_1.json"));
            PageContent content = new PageContent(contentType, json);

            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler(Utilities.DefaultLogFactory);
            BeatSaverLatestSettings settings = new BeatSaverLatestSettings()
            {
                StoreRawData = true
            };
            PageReadResult result = pageHandler.Parse(content, uri, settings);

            Assert.IsTrue(result.Successful);
            Assert.IsNull(result.Exception);
            Assert.AreEqual(expectedPageError, result.PageError);
            Assert.AreEqual(expectedIsLastPage, result.IsLastPage);
            Assert.AreEqual(expectedSongs, result.SongCount);
            Assert.AreEqual(expectedSongsOnPage, result.SongsOnPage);
            foreach (var song in result.Songs())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Hash), "Hash is empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Key), "Key is empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.LevelAuthorName), "LevelAuthorName is empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.RawData), "RawData is empty");
                Assert.AreEqual(uri.ToString(), song.SourceUri?.ToString());
                Assert.IsTrue(song.DownloadUri?.ToString().ToUpper().Contains(song.Hash!),
                    $"{song.DownloadUri}");
            }
        }
    }
}
