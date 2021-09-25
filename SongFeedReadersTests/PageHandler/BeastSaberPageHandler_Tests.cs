using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.BeastSaber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SongFeedReadersTests.PageHandler
{
    [TestClass]
    public class BeastSaberPageHandler_Tests
    {
        readonly string DataPath = Path.GetFullPath(Path.Combine("Data", "BeastSaber"));
        string GetFilePath(string fileName) => Path.Combine(DataPath, fileName);
        [TestMethod]
        public void StandardPage()
        {
            int expectedSongs = 50;
            int expectedSongsOnPage = 50;
            PageErrorType expectedPageError = PageErrorType.None;
            bool expectedIsLastPage = false;

            Uri uri = new Uri(@"https://bsaber.com/wp-json/bsaber-api/songs/?followed_by=Zingabopp&count=50&page=1");
            string contentType = PageContent.ContentId_JSON;
            string json = File.ReadAllText(GetFilePath("bsaber_follows_1.json"));
            PageContent content = new PageContent(contentType, json);

            BeastSaberPageHandler pageHandler = new BeastSaberPageHandler(Utilities.DefaultLogFactory);
            BeastSaberFollowsSettings settings = new BeastSaberFollowsSettings()
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
