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
        private readonly IFeedFactory _feeds;
        public IFeed? SelectedFeed
        {
            get { return _selectedFeed; }
            set
            {
                if (_selectedFeed == value) return;
                _selectedFeed = value;
                if (value != null)
                {
                    FeedViewModel = new FeedViewModel(value);
                }
                else
                    FeedViewModel = null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FeedViewModel));
            }
        }

        public FeedViewModel? FeedViewModel { get; protected set; }

        public ObservableCollection<IFeed> Feeds { get; }

        public NavigationViewModel(IFeedFactory feedFactory, IEnumerable<IFeedSettings> settings)
        {
            _feeds = feedFactory ?? throw new ArgumentNullException(nameof(feedFactory));
            
            Feeds = new ObservableCollection<IFeed>(settings.Select(s => feedFactory.GetFeed(s)));
            IFeed? first = Feeds.FirstOrDefault();
            if (first != null)
                SelectedFeed = first;
        }

    }
}
