using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongFeedReaders.Feeds;
using SongFeedReaders.Feeds.ScoreSaber;
using SongFeedReaders.Models;
using System;
using System.IO;

namespace SongFeedReadersTests.PageHandler
{
    [TestClass]
    public class ScoreSaberPageHandler_Tests
    {
        private readonly string DataPath = Path.GetFullPath(Path.Combine("Data", "ScoreSaber"));

        private string GetFilePath(string fileName) => Path.Combine(DataPath, fileName);
        [TestMethod]
        public void StandardPage()
        {
            int expectedSongs = 50;
            int expectedSongsOnPage = 50;
            PageErrorType expectedPageError = PageErrorType.None;
            bool expectedIsLastPage = false;

            Uri uri = new Uri(@"https://scoresaber.com/api.php?function=get-leaderboards&cat=1&limit=50&page=1&ranked=1");
            string contentType = "application/json";
            string json = File.ReadAllText(GetFilePath("scoresaber_latest_1.json"));
            PageContent content = new PageContent(contentType, json);

            ScoreSaberPageHandler pageHandler = new ScoreSaberPageHandler(Utilities.DefaultLogFactory);
            ScoreSaberLatestSettings settings = new ScoreSaberLatestSettings()
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
            foreach (SongFeedReaders.Models.ScrapedSong? song in result.Songs())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Hash), "Hash is empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.LevelAuthorName), "LevelAuthorName is empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.RawData), "RawData is empty");
                Assert.AreEqual(uri.ToString(), song.SourceUri?.ToString());
                Assert.IsTrue(song.DownloadUri?.ToString().ToUpper().Contains(song.Hash!),
                    $"{song.DownloadUri}");
                if (song is ScoreSaberSong sSong)
                {
                    Assert.IsTrue(sSong.ScoreSaberId > 0);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(sSong.Difficulty));
                    Assert.IsFalse(string.IsNullOrWhiteSpace(sSong.SongAuthorName));
                    Assert.IsTrue(sSong.BPM > 0);
                    Assert.IsTrue(sSong.Scores > 0, $"Scores = 0 for {sSong.ScoreSaberId}");
                    Assert.IsTrue(sSong.ScoresPerDay > 0);
                    Assert.IsTrue(sSong.Ranked);
                    Assert.IsTrue(sSong.Stars > 0);
                }
                else
                    Assert.Fail("song is not a ScoreSaberSong");
            }
        }
    }
}
