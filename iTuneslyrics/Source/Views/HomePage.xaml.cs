using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using iTuneslyrics.Source.ViewModels;

namespace iTuneslyrics.Source.Views
{
    public partial class HomePage : Page
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(1.5);

        private readonly HomeViewModel _viewModel;
        private readonly DispatcherTimer _pollTimer;
        private Window _window;

        public HomePage(HomeViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();

            // iTunes has no selection-changed event, so watch for changes two ways while Home is shown:
            // re-read on window activation (instant when the user clicks back in), and poll a cheap
            // fingerprint on a timer (covers always-on-top, where the user never clicks back in).
            _pollTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = PollInterval };
            _pollTimer.Tick += OnPollTick;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Refresh();

            _window = Window.GetWindow(this);
            if (_window != null)
                _window.Activated += OnWindowActivated;

            _pollTimer.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _pollTimer.Stop();

            if (_window != null)
                _window.Activated -= OnWindowActivated;
            _window = null;
        }

        private void OnWindowActivated(object sender, EventArgs e)
        {
            _viewModel.RefreshSelectionQuietly();
        }

        private void OnPollTick(object sender, EventArgs e)
        {
            // Nothing to show while minimized; activation will catch up on restore.
            if (_window?.WindowState == WindowState.Minimized)
                return;

            _viewModel.PollSelection();
        }
    }
}
