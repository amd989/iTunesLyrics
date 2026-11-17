using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using iTuneslyrics.Source.ViewModels;

namespace iTuneslyrics.Source.Views
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow(
            MainViewModel viewModel,
            INavigationService navigationService,
            INavigationViewPageProvider pageProvider)
        {
            DataContext = viewModel;
            InitializeComponent();

            NavigationView.SetPageProviderService(pageProvider);
            navigationService.SetNavigationControl(NavigationView);

            Loaded += (_, _) => NavigationView.Navigate(typeof(HomePage));

            // Watch for OS theme changes and apply automatically
            SystemThemeWatcher.Watch(this);

            // Bind always-on-top
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsAlwaysOnTop))
                    Topmost = viewModel.IsAlwaysOnTop;
            };
            Topmost = viewModel.IsAlwaysOnTop;
        }
    }
}
