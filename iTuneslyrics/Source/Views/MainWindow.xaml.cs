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
        private readonly MainViewModel _viewModel;

        public MainWindow(
            MainViewModel viewModel,
            INavigationService navigationService,
            INavigationViewPageProvider pageProvider)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();

            NavigationView.SetPageProviderService(pageProvider);
            navigationService.SetNavigationControl(NavigationView);

            NavigationView.Navigated += OnNavigated;
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

        private void OnNavigated(NavigationView sender, NavigatedEventArgs args)
        {
            if (_viewModel.PendingDataContext == null)
                return;

            // The NavigationView renders the page in its content presenter.
            // After navigation, find the page from the sender's visual tree.
            if (args.Page is FrameworkElement page)
            {
                page.DataContext = _viewModel.PendingDataContext;
            }
            _viewModel.PendingDataContext = null;
        }
    }
}
