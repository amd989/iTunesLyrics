using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTuneslyrics.Source.Services;

namespace iTuneslyrics.Source.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IUserSettings _settings;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private string _apiToken;

        [ObservableProperty]
        private bool _isAlwaysOnTop;

        [ObservableProperty]
        private string _configFilePath;

        public SettingsViewModel(IUserSettings settings, MainViewModel mainViewModel)
        {
            _settings = settings;
            _mainViewModel = mainViewModel;
            _apiToken = settings.GeniusApiToken;
            _isAlwaysOnTop = settings.AlwaysOnTop;
            _configFilePath = settings.ConfigFilePath;
        }

        [RelayCommand]
        private void Save()
        {
            _settings.GeniusApiToken = ApiToken?.Trim() ?? string.Empty;
            _settings.AlwaysOnTop = IsAlwaysOnTop;
            _mainViewModel.IsAlwaysOnTop = IsAlwaysOnTop;
        }

        partial void OnIsAlwaysOnTopChanged(bool value)
        {
            _settings.AlwaysOnTop = value;
            _mainViewModel.IsAlwaysOnTop = value;
        }
    }
}
