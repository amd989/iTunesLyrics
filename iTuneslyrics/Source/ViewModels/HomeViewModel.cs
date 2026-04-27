using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTunesLib;
using iTuneslyrics.Source.Services;
using iTuneslyrics.Source.Views;

namespace iTuneslyrics.Source.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IITunesService _itunes;
        private readonly IUserSettings _settings;
        private readonly IGeniusService _genius;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private bool _isAutoUpdate = true;

        [ObservableProperty]
        private bool _isOverwrite;

        [ObservableProperty]
        private int _trackCount;

        [ObservableProperty]
        private bool _isITunesAvailable;

        [ObservableProperty]
        private string _errorMessage;

        private IReadOnlyList<IITFileOrCDTrack> _selectedTracks;

        public HomeViewModel(IITunesService itunes, IUserSettings settings, IGeniusService genius, MainViewModel mainViewModel)
        {
            _itunes = itunes;
            _settings = settings;
            _genius = genius;
            _mainViewModel = mainViewModel;
        }

        public void Refresh()
        {
            IsITunesAvailable = _itunes.IsAvailable;
            if (!IsITunesAvailable)
            {
                ErrorMessage = "iTunes is not installed or not responding.";
                TrackCount = 0;
                return;
            }

            ErrorMessage = null;
            try
            {
                _itunes.EnsureVisible();
                _selectedTracks = _itunes.GetSelectedTracks();
                TrackCount = _selectedTracks.Count;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Could not read iTunes selection: " + ex.Message;
                TrackCount = 0;
            }
        }

        [RelayCommand]
        private void GetLyrics()
        {
            var token = _settings.GeniusApiToken;
            if (string.IsNullOrWhiteSpace(token))
            {
                _mainViewModel.Navigate(typeof(SettingsPage));
                return;
            }

            if (_selectedTracks == null || _selectedTracks.Count == 0)
                return;

            if (IsAutoUpdate)
            {
                var vm = new ResultsViewModel(_genius, _itunes, _mainViewModel);
                vm.Start(_selectedTracks, IsOverwrite);
                _mainViewModel.Navigate(typeof(ResultsPage), vm);
            }
            else
            {
                var vm = new ReviewViewModel(_genius, _itunes, _mainViewModel);
                vm.LoadQueue(_selectedTracks);
                _mainViewModel.Navigate(typeof(ReviewPage), vm);
            }
        }
    }
}
