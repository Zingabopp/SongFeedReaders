using SongFeedReaders.Feeds;

namespace SongFeedReaders.Services
{
    /// <summary>
    /// Interface for retrieving stored <see cref="IFeedSettings"/>.
    /// </summary>
    public interface ISettingsFactory
    {
        /// <summary>
        /// Gets the <see cref="IFeedSettings"/> with the specified <paramref name="feedId"/>.
        /// </summary>
        /// <param name="feedId"></param>
        /// <returns></returns>
        IFeedSettings? GetSettings(string feedId);
        /// <summary>
        /// Removes and returns the specified settings, if they exist.
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <param name="feedId"></param>
        /// <returns></returns>
        TSettings? RemoveSettings<TSettings>(string? feedId = null)
            where TSettings : class, IFeedSettings;
    }
}
