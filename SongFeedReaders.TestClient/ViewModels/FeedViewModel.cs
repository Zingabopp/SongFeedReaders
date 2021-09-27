using MVVM;
using SongFeedReaders.Feeds;
using SongFeedReaders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SongFeedReaders.TestClient.ViewModels
{
    public class FeedViewModel : ViewModelBase
    {
        public IFeed Feed { get; }
        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (_loading == value) return;
                _loading = value;
                NotifyPropertyChanged();
                RefreshActions();
            }
        }
        private PageReadResult? _currentResult;

        public PageReadResult? CurrentResult
        {
            get { return _currentResult; }
            set
            {
                if (_currentResult == value) return;
                _currentResult = value;
                NotifyPropertyChanged();
                RefreshActions();
                if (value != null && value.Successful)
                    Songs = value.Songs().ToArray();
                NotifyPropertyChanged(nameof(Songs));
            }
        }

        public ScrapedSong[]? Songs { get; protected set; }

        public bool CanMoveNext => feedEnumerator?.CanMoveNext ?? false;
        public bool CanMovePrevious => feedEnumerator?.CanMovePrevious ?? false;

        private void RefreshActions()
        {
            Refresh.RaiseCanExecuteChanged();
            NextPage.RaiseCanExecuteChanged();
            PreviousPage.RaiseCanExecuteChanged();
            NotifyPropertyChanged(nameof(CanMoveNext));
            NotifyPropertyChanged(nameof(CanMovePrevious));
        }

        private FeedAsyncEnumerator? feedEnumerator;
        public FeedViewModel(IFeed feed)
        {
            Feed = feed ?? throw new ArgumentNullException(nameof(feed));
            Refresh = new RelayCommand(async () =>
            {
                if (Loading)
                    return;
                Loading = true;
                try
                {
                    if (!feed.Initialized)
                        await feed.InitializeAsync(CancellationToken.None);
                    feedEnumerator = feed.GetAsyncEnumerator();
                    CurrentResult = await feedEnumerator.MoveNextAsync();
                }
                finally
                {
                    Loading = false;
                }
            }, () => !Loading, true);
            NextPage = new RelayCommand(async () =>
            {
                FeedAsyncEnumerator? fe = feedEnumerator;
                if (fe == null || Loading || !fe.CanMoveNext)
                    return;
                Loading = true;
                try
                {
                    CurrentResult = await fe.MoveNextAsync();
                }
                finally
                {
                    Loading = false;
                }
            }, () => !Loading && CanMoveNext, true);
            PreviousPage = new RelayCommand(async () =>
            {
                FeedAsyncEnumerator? fe = feedEnumerator;
                if (fe == null || Loading || !fe.CanMovePrevious)
                    return;
                Loading = true;
                try
                {
                    CurrentResult = await fe.MovePreviousAsync();
                }
                finally
                {
                    Loading = false;
                }
            }, () => !Loading && CanMovePrevious, true);
            Refresh.Execute();
        }


        public RelayCommand Refresh { get; }
        public RelayCommand NextPage { get; }
        public RelayCommand PreviousPage { get; }
    }
}
