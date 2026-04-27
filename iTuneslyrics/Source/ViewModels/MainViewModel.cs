using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui;

namespace iTuneslyrics.Source.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isAlwaysOnTop;

        public object PendingDataContext { get; set; }

        public MainViewModel(INavigationService navigationService, Services.IUserSettings settings)
        {
            _navigationService = navigationService;
            _isAlwaysOnTop = settings.AlwaysOnTop;
        }

        public void Navigate(Type pageType, object dataContext = null)
        {
            PendingDataContext = dataContext;
            _navigationService.Navigate(pageType);
        }
    }
}
