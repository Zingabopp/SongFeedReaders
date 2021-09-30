using System;
using System.Collections.Generic;
using System.Text;

namespace SongFeedReaders.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class FeedAttribute : Attribute
    {
        readonly Type settingsType;

        public FeedAttribute(Type settingsType)
        {
            this.settingsType = settingsType;


        }

        public Type SettingsType
        {
            get { return settingsType; }
        }
    }
}
