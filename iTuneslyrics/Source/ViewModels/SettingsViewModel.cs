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

        /// <summary>True when no token is saved. Drives the prompt shown when Home redirects here.</summary>
        [ObservableProperty]
        private bool _isTokenMissing;

        [ObservableProperty]
        private bool _isSaved;

        public SettingsViewModel(IUserSettings settings, MainViewModel mainViewModel)
        {
            _settings = settings;
            _mainViewModel = mainViewModel;
            _apiToken = settings.GeniusApiToken;
            _isAlwaysOnTop = settings.AlwaysOnTop;
            _configFilePath = settings.ConfigFilePath;
            _isTokenMissing = string.IsNullOrWhiteSpace(_apiToken);
        }

        [RelayCommand]
        private void Save()
        {
            var token = ApiToken?.Trim() ?? string.Empty;
            _settings.GeniusApiToken = token;
            IsTokenMissing = token.Length == 0;
            IsSaved = !IsTokenMissing;
        }

        partial void OnApiTokenChanged(string value)
        {
            IsSaved = false;
        }

        partial void OnIsAlwaysOnTopChanged(bool value)
        {
            _settings.AlwaysOnTop = value;
            _mainViewModel.IsAlwaysOnTop = value;
        }
    }
}
