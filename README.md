# SongFeedReaders
This library was created for BeatSync to handle reading collections of Beat Saber beatmaps from various sources, but it should be usable and expandable for other projects. The class design follows Dependency Injection principles and using some kind of DI framework is recommended for constructing the services.

# Usage
## IFeed
The central part of this library is the `IFeed` interface. It represents a specific feed for a service. The `SongFeedReaders.TestClient` project is an example of how to use the library with WPF and Microsoft's Dependency Injection library.
### IFeed Required Services
The feed classes implemented in this library require the following services: `IFeedSettings`/`ISettingsFactory`, `IFeedPageHandler`, `IWebClient`.
* `IFeedPageHandler`: `IFeedPageHandler` must be a type specific to the feed service (i.e. All Beat Saver feeds require an `IBeatSaverPageHandler`). The `IFeedPageHandler` implementations in this library can safely be shared by multiple feeds.
* `IWebClient`: The web client used by the feeds. A single instance can and should be shared by all the feeds.
  * `IWebClient` is provided by the [WebUtilities](https://github.com/Zingabopp/WebUtilities) project.
### IFeed Optional Service(s)
* `ILogFactory`: Provide this service to receive log messages.
### Notable Properties/Methods
* `TryAssignSettings(IFeedSettings): Assigns settings to the feed.
  * `IFeedSettings` must be a type specific to the feed (i.e. `BeatSaverLatestFeed` requires `BeatSaverLatestSettings`).
  * Returns true if the settings were accepted. Any calls after settings have been set will return false and not change the existing settings.
* `InitializeAsync()`: This is required to be called at least once for each feed. Some feeds require this to be called for initial setup (i.e. BeatSaver Mapper needs to do an API request to get the Beat Saver uploader ID using the username given in the settings). Repeated calls should be handled gracefully by implementations.
* `Initialized`: A boolean property that indicates if `InitializeAsync()` was called and finished successfully.
* `ReadAsync()`: This will attempt to read the entire feed according to the current settings and return a `FeedResult`.
* `GetAsyncEnumerator()`: This returns a `FeedAsyncEnumerator` that can be used to page forward or backward through a feed according to the `IFeedSettings`.
### Warnings
* Changing a feed's settings while it is running or while a `FeedAsyncEnumerator` is active could cause unexpected behavior and is not recommended.
* Most exceptions thrown will be caught and stored in the `PageReadResult` or `FeedResult` returned by the methods. Be sure to check for and provide feedback to the user if necessary. Some exceptions may be thrown immediately, check the documentation/IntelliSense to see what needs to be handled by calling code.

# Development
This library was designed to easily incorporate additional feeds and services.
## Creating New Feeds
New feeds can be added by implementing the following interfaces:
* `IFeedSettings`: A settings class/interface specific to your feed.
* `IFeedPageHandler`: This component handles parsing page content into a `PageReadResult`. If none of the existing implementations work for your feed, you'll need to create your own.
  * Your page handler should make a best attempt to detect when the last page of a feed is reached.
  * The base class `FeedPageHandlerBase` can be used to handle basic details, such as filtering out songs based on `IFeedSettings.Filter` and `IFeedSettings.StopWhenAny` and will automatically set `PageReadResult.IsLastPage` if no songs were listed on the page.
* `IFeed`: Most feeds should implement either `IPagedFeed` or `IDatedFeed` to be compatible with existing `FeedAsyncEnumerators`. Paged feeds are the most common; they are feeds that include a page number in the URL. Dated feeds are used when the URL uses a datetime instead of a page number (i.e. Beat Saver Latest feed). **Most feeds should be able to extend `FeedBase` to minimize the amount of code needed to implement `IFeed`.**
  * Properties:
    * `FeedId`: A string ID uniquely identifying your feed. The form `ServiceName.FeedName` is recommended.
    * `DisplayName`: A human-friendly display name for your feed.
    * `Description`: A description of your feed.
    * `FeedStartingPage` (`IPagedFeed` only): Will likely be a '1' or '0', depending on what the service uses as its first page number.
  * Methods:
    * `GetUriForPage(int page)` (`IPagedFeed` only): Gets the web URI for the specified page.
    * `bool AreSettingsValid(IFeedSettings settings`: Return true if the given settings are valid for the feed, false otherwise.
    * `FeedAsyncEnumerator GetAsyncEnumerator(IFeedSettings settings)`: Ensure the settings are valid and return a new `FeedAsyncEnumerator`.
