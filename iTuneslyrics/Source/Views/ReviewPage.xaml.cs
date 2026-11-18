using System.Windows.Controls;
using iTuneslyrics.Source.ViewModels;

namespace iTuneslyrics.Source.Views
{
    public partial class ReviewPage : Page
    {
        public ReviewPage(ReviewViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
