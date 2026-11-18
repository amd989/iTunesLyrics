using CommunityToolkit.Mvvm.ComponentModel;

namespace iTuneslyrics.Source.Models
{
    public partial class TrackResult : ObservableObject
    {
        [ObservableProperty]
        private string _song;

        [ObservableProperty]
        private string _artist;

        [ObservableProperty]
        private ResultStatus _status;
    }
}
