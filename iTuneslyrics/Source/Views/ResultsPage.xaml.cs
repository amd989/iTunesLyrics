using System.ComponentModel;
using System.Windows.Controls;
using iTuneslyrics.Source.ViewModels;

namespace iTuneslyrics.Source.Views
{
    public partial class ResultsPage : Page
    {
        private readonly ResultsViewModel _viewModel;

        public ResultsPage(ResultsViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();

            // The VM is a singleton and outlives this page; subscribe only while visible to avoid leaking pages.
            Loaded += (_, _) =>
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                ScrollToCurrent();
            };
            Unloaded += (_, _) => _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ResultsViewModel.CurrentTrack))
                ScrollToCurrent();
        }

        private void ScrollToCurrent()
        {
            if (_viewModel.CurrentTrack != null)
                ResultsGrid.ScrollIntoView(_viewModel.CurrentTrack);
        }
    }
}
