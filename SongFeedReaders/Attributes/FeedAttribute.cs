using SongFeedReaders.Feeds;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Attributes
{
    /// <summary>
    /// Identifies a class as an <see cref="IFeed"/> and specifies the <see cref="IFeedSettings"/> type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class FeedAttribute : Attribute
    {
        readonly Type settingsType;

        /// <summary>
        /// Identifies a class as an <see cref="IFeed"/> and specifies the <see cref="IFeedSettings"/> type.
        /// </summary>
        /// <param name="settingsType"></param>
        public FeedAttribute(Type settingsType)
        {
            this.settingsType = settingsType;


        }
        /// <summary>
        /// Type of <see cref="IFeedSettings"/> used by this feed.
        /// </summary>
        public Type SettingsType
        {
            get { return settingsType; }
        }
    }
}
