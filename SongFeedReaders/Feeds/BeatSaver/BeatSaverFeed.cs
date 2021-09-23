﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SongFeedReaders.Logging;
using SongFeedReaders.Models;
using SongFeedReaders.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUtilities;

namespace SongFeedReaders.Feeds.BeatSaver
{
    /// <summary>
    /// Base class for Beat Saver feeds.
    /// </summary>
    public abstract class BeatSaverFeed : FeedBase
    {
        /// <summary>
        /// Initializes a new <see cref="BeatSaverFeed"/>.
        /// </summary>
        /// <param name="feedSettings"></param>
        /// <param name="pageHandler"></param>
        /// <param name="webClient"></param>
        /// <param name="logFactory"></param>
        protected BeatSaverFeed(IFeedSettings feedSettings, IBeatSaverPageHandler pageHandler,
            IWebClient webClient, ILogFactory? logFactory)
            : base(feedSettings, pageHandler, webClient, logFactory)
        {
        }
        /// <inheritdoc/>
        protected override async Task<PageContent> GetPageContent(IWebResponseContent responseContent)
        {
            string pageText = await responseContent.ReadAsStringAsync().ConfigureAwait(false);
            return new PageContent(PageContent.ContentId_JSON, pageText);
        }
    }
}