using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeatSaver;
using SongFeedReaders.Models;
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
            foreach (ScrapedSong? song in result.Songs())
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

        [TestMethod]
        public void ParseSingle_HasKey()
        {
            Uri uri = new Uri(@"https://api.beatsaver.com/maps/id/1");
            string filePath = GetFilePath("single_1.json");
            string pageText = File.ReadAllText(filePath);
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler(Utilities.DefaultLogFactory);

            string expectedHash = "fda568fc27c20d21f8dc6f3709b49b5cc96723be".ToUpper();
            string expectedKey = "1";
            string expectedName = "me & u";
            string expectedLevelAuthorName = "datkami";
            string expectedDownloadUrl = "https://na.cdn.beatsaver.com/fda568fc27c20d21f8dc6f3709b49b5cc96723be.zip";

            ScrapedSong? song = pageHandler.ParseSingle(pageText, uri, true);

            Assert.AreEqual(expectedHash, song.Hash);
            Assert.AreEqual(expectedKey, song.Key);
            Assert.AreEqual(expectedName, song.Name);
            Assert.AreEqual(expectedLevelAuthorName, song.LevelAuthorName);
            Assert.AreEqual(expectedDownloadUrl, song.DownloadUri?.ToString());
            Assert.AreEqual(uri.ToString(), song.SourceUri?.ToString());
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.RawData));
        }

        [TestMethod]
        public void ParseSingle_NoKey() // No version ID, just Beat Saver ID
        {
            Uri uri = new Uri(@"https://api.beatsaver.com/maps/id/1c82c");
            string filePath = GetFilePath("single_2.json");
            string pageText = File.ReadAllText(filePath);
            BeatSaverPageHandler pageHandler = new BeatSaverPageHandler(Utilities.DefaultLogFactory);

            string expectedHash = "1cf39eea882ec6bd02dcd2ee889139fbac2a6076".ToUpper();
            string expectedKey = "1c82c".ToUpper();
            string expectedName = "Ghost";
            string expectedLevelAuthorName = "codfish1738";
            string expectedDownloadUrl = "https://na.cdn.beatsaver.com/1cf39eea882ec6bd02dcd2ee889139fbac2a6076.zip";

            ScrapedSong? song = pageHandler.ParseSingle(pageText, uri, true);

            Assert.AreEqual(expectedHash, song.Hash);
            Assert.AreEqual(expectedKey, song.Key);
            Assert.AreEqual(expectedName, song.Name);
            Assert.AreEqual(expectedLevelAuthorName, song.LevelAuthorName);
            Assert.AreEqual(expectedDownloadUrl, song.DownloadUri?.ToString());
            Assert.AreEqual(uri.ToString(), song.SourceUri?.ToString());
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.RawData));
        }
    }
}
