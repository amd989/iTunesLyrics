using System.Windows.Controls;
using iTuneslyrics.Source.ViewModels;

namespace iTuneslyrics.Source.Views
{
    public partial class ResultsPage : Page
    {
        public ResultsPage(ResultsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
