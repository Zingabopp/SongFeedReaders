using SongFeedReaders.Feeds;
using SongFeedReaders.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SongFeedReaders.TestClient.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private IFeed? _selectedFeed;
        private readonly ISettingsFactory _settingsFactory;
        public IFeed? SelectedFeed
        {
            get { return _selectedFeed; }
            set
            {
                if (_selectedFeed == value) return;
                _selectedFeed = value;
                if (value != null)
                {
                    IFeedSettings? settings = _settingsFactory.GetSettings(value.FeedId);
                    FeedViewModel = new FeedViewModel(value, settings);
                }
                else
                    FeedViewModel = null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FeedViewModel));
            }
        }

        public FeedViewModel? FeedViewModel { get; protected set; }

        public ObservableCollection<IFeed> Feeds { get; }

        public NavigationViewModel(IEnumerable<IFeed> feeds, ISettingsFactory settingsFactory)
        {
            _settingsFactory = settingsFactory ?? throw new ArgumentNullException(nameof(settingsFactory));
            Feeds = new ObservableCollection<IFeed>(feeds);
            IFeed? first = Feeds.FirstOrDefault();
            if (first != null)
                SelectedFeed = first;
        }

    }
}
