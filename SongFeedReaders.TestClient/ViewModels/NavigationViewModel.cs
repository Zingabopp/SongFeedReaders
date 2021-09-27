using SongFeedReaders.Feeds;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SongFeedReaders.TestClient.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private IFeed? _selectedFeed;

        public IFeed? SelectedFeed
        {
            get { return _selectedFeed; }
            set
            {
                if (_selectedFeed == value) return;
                _selectedFeed = value;
                if (value != null)
                    FeedViewModel = new FeedViewModel(value);
                else
                    FeedViewModel = null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FeedViewModel));
            }
        }

        public FeedViewModel? FeedViewModel { get; protected set; }

        public ObservableCollection<IFeed> Feeds { get; }

        public NavigationViewModel(IEnumerable<IFeed> feeds)
        {
            Feeds = new ObservableCollection<IFeed>(feeds);
            IFeed? first = Feeds.FirstOrDefault();
            if (first != null)
                SelectedFeed = first;
        }

    }
}
