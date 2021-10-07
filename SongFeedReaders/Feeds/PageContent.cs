namespace SongFeedReaders.Feeds
{

    /// <summary>
    /// Container for page content and its type.
    /// </summary>
    public struct PageContent
    {
        /// <summary>
        /// ContentId for when it's unrecognized.
        /// </summary>
        public static readonly string ContentId_Unknown = "Unknown";
        /// <summary>
        /// ContentId for when it's not specified.
        /// </summary>
        public static readonly string ContentId_NotSpecified = "NotSpecified";
        /// <summary>
        /// <see cref="ContentId"/> for XML.
        /// </summary>
        public static readonly string ContentId_XML = "XML";
        /// <summary>
        /// <see cref="ContentId"/> for JSON.
        /// </summary>
        public static readonly string ContentId_JSON = "JSON";
        /// <summary>
        /// Type of content on the page.
        /// </summary>
        public readonly string ContentId;
        /// <summary>
        /// Page content.
        /// </summary>
        public readonly string Content;
        /// <summary>
        /// Constructs a <see cref="PageContent"/>.
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="content"></param>
        public PageContent(string contentId, string? content)
        {
            ContentId = string.IsNullOrWhiteSpace(contentId) ? ContentId_NotSpecified : contentId;
            Content = content ?? string.Empty;
        }
    }
}
