using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using WebUtilities;
using WebUtilities.HttpClientWrapper;
using WebUtilities.Mock.MockClient;

namespace SongFeedReadersTests
{
    internal static class MockClientSetup
    {
        public static string UriToPath(Uri uri)
        {
            string url = uri.ToString();
            string server = uri.Host;
            string query = uri.Query;
            if (server == "api.beatsaver.com")
                return BeatSaverUriToPath(uri);
            else if (server == "bsaber.com")
                return BeastSaberUriToPath(uri);
            else if (server == "scoresaber.com")
                return ScoreSaberUriToPath(uri);
            throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri));
        }

        private static string BeatSaverUriToPath(Uri uri)
        {
            string path = "BeatSaver";
            string localPath = uri.LocalPath.TrimStart('/');
            string[] parts = localPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (localPath.StartsWith("users/name/"))
            {
                path = Path.Combine(path, "userquery");
                string name = uri.LocalPath.Split('/').Last();
                path = Path.Combine(path, name);
            }
            else if (localPath.StartsWith("maps/uploader"))
            {
                path = Path.Combine(path, "usermaps");
                string uploaderId = parts[2];
                string page = parts[3];
                path = Path.Combine(path, uploaderId, $"{uploaderId}-{page}");
            }
            else if (localPath.StartsWith("maps/latest"))
            {
                path = Path.Combine(path, "latest");
                var queries = GetQueries(uri);
                string? before = queries["before"];
                string? after = queries["after"];
                
                string prefix = queries.Get("sort") == "CURATED" ? "curated" : "latest";
                path = Path.Combine(path, $"{prefix}-b={before}a={after}".Replace(':', '_'));
            }
            else
            {
                throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri));
            }
            path = path + ".json";
            return path;
        }

        private static NameValueCollection GetQueries(Uri uri)
        {
            string query = uri.Query;
            var collection = System.Web.HttpUtility.ParseQueryString(query);
            return collection;
        }

        private static string ScoreSaberUriToPath(Uri uri)
        {
            string path = "ScoreSaber";
            string localPath = uri.LocalPath.TrimStart('/');
            string[] parts = localPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var queries = GetQueries(uri);
            string? feedNumber = queries["cat"];
            string feedName = feedNumber switch
            {
                "0" => "Trending",
                "1" => "Latest",
                "2" => "TopPlayed",
                "3" => "TopRanked",
                _ => throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri))
            };
            string? page = queries["page"];
            string rankedOnly = queries["ranked"] == "1" ? "RankedOnly" : "";
            string limit = queries["limit"];
            if (limit != null)
                limit = $"limit={limit}";
            path = Path.Combine(path, feedName, $"{feedName}-{string.Join("_", rankedOnly, limit)}_{page}");
            path = path + ".json";
            return path;
        }
        private static string BeastSaberUriToPath(Uri uri)
        {
            string path = "BeastSaber";
            string localPath = uri.LocalPath.TrimStart('/');
            var queries = GetQueries(uri);
            string? bookmarked_by = queries["bookmarked_by"];
            string? followed_by = queries["followed_by"];
            string username;
            string feedName;
            if (bookmarked_by != null)
            {
                feedName = "bookmarks";
                username = bookmarked_by;
            }
            else if (followed_by != null)
            {
                feedName = "follows";
                username = followed_by;
            }
            else
                throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri));
            string? page = queries["page"] ?? "";
            string count = queries["count"];
            if (count != null)
                count = $"count={count}";
            path = Path.Combine(path, feedName, username, $"{username}-{string.Join("_", count, page)}");
            path = path + ".json";
            return path;
        }

        public static IWebClient GetMockClient()
        {
            Directory.CreateDirectory("ResponseData");
            return new MockClient("ResponseData", UriToPath)
#if !NCRUNCH
            //.WithRecordingClient(new HttpClientWrapper("SongFeedReaders.Tests/1.0.0"));
#endif
                ;
        }


    }
}
