using SongFeedReaders.Feeds;
using SongFeedReaders.Services;
using System;

namespace SongFeedReaders.TestClient
{
    internal class TestFeedFactory : FeedFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;
        public TestFeedFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            int feedsRegistered = AddAttributedFeeds(typeof(IFeed).Assembly);
            Console.WriteLine($"{feedsRegistered} feeds auto-registered.");
        }

        protected override IFeed? InstantiateFeed(Type feedType)
        {
            return _serviceProvider.GetService(feedType) as IFeed;
        }
    }
}
