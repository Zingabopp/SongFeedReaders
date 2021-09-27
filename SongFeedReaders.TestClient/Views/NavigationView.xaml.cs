﻿using SongFeedReaders.TestClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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