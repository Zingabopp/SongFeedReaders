namespace SongFeedReaders.Logging
{
    /// <summary>
    /// A factory to create <see cref="ILogger"/>.
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Gets an <see cref="ILogger"/>, optionally with a module name.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        ILogger GetLogger(string? moduleName = null);

        /// <summary>
        /// Gets an <see cref="ILogger"/> using <typeparamref name="T"/> as the module name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ILogger GetLogger<T>();
    }
}
