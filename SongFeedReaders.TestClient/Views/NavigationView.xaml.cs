using SongFeedReaders.TestClient.ViewModels;
using System.Windows;

namespace SongFeedReaders.TestClient.Views
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : Window
    {
        public NavigationView(NavigationViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
