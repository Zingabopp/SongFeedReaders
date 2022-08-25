using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using WebUtilities;
using WebUtilities.HttpClientWrapper;
using WebUtilities.Mock.MockClient;

namespace SongFeedReadersTests
{
    internal static class MockClientSetup
    {
        /// <summary>
        /// Set to true to record live data to disk.
        /// Response data is saved to the output folder and must be copied to the project's data folder.
        /// </summary>
        public static bool UseRecordingClient = false;
        public static string UriToPath(Uri uri)
        {
            string server = uri.Host;
            if (server == "api.beatsaver.com")
                return BeatSaverUriToPath(uri);
            else if (server == "bsaber.com")
                return BeastSaberUriToPath(uri);
            else if (server == "scoresaber.com")
                return ScoreSaberUriToPath(uri);
            else if (server == "raw.githubusercontent.com")
                return GithubUserContent(uri);
            throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri));
        }

        public static string GithubUserContent(Uri uri)
        {
            string path = "GithubUserContent";
            string localPath = uri.LocalPath.TrimStart('/');
            string[] parts = localPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                throw new Exception("Path for GitHubUserContent in unexpected format");
            string user = parts[0];
            string repo = parts[1];
            string branch = parts[2];
            string file = parts[3];
            path = Path.Combine(path, user, repo, branch, file);
            return path;
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
                NameValueCollection queries = GetQueries(uri);
                string? before = queries["before"];
                string? after = queries["after"];

                string prefix = queries.Get("sort") == "CURATED" ? "curated" : "latest";
                path = Path.Combine(path, $"{prefix}-b={before}a={after}".Replace(':', '_'));
            }
            else
            {
                throw new ArgumentException($"Could not create a path for Uri '{uri}'", nameof(uri));
            }
            path += ".json";
            return path;
        }

        private static NameValueCollection GetQueries(Uri uri)
        {
            string query = uri.Query;
            NameValueCollection collection = System.Web.HttpUtility.ParseQueryString(query);
            return collection;
        }

        private static string ScoreSaberUriToPath(Uri uri)
        {
            string path = "ScoreSaber";
            NameValueCollection queries = GetQueries(uri);
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
            path += ".json";
            return path;
        }
        private static string BeastSaberUriToPath(Uri uri)
        {
            string path = "BeastSaber";
            NameValueCollection queries = GetQueries(uri);
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
            path += ".json";
            return path;
        }

        public static IWebClient GetMockClient()
        {
            Directory.CreateDirectory("ResponseData");
            MockClient client = new MockClient("ResponseData", UriToPath);
#if !NCRUNCH
            if (UseRecordingClient)
                client = client.WithRecordingClient(new HttpClientWrapper("SongFeedReaders.Tests/1.0.0"));
#endif
            return client;
        }


    }
}
