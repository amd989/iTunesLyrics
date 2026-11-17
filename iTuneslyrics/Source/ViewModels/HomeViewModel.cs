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
        private readonly MainViewModel _mainViewModel;
        private readonly ResultsViewModel _results;
        private readonly ReviewViewModel _review;

        [ObservableProperty]
        private bool _isAutoUpdate = true;

        [ObservableProperty]
        private bool _isOverwrite;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectionText))]
        [NotifyCanExecuteChangedFor(nameof(GetLyricsCommand))]
        private int _trackCount;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GetLyricsCommand))]
        private bool _isITunesAvailable;

        [ObservableProperty]
        private string _errorMessage;

        private IReadOnlyList<IITFileOrCDTrack> _selectedTracks = Array.Empty<IITFileOrCDTrack>();

        public string SelectionText => TrackCount switch
        {
            0 => "No tracks selected",
            1 => "1 track",
            _ => $"{TrackCount} tracks"
        };

        public HomeViewModel(
            IITunesService itunes,
            IUserSettings settings,
            MainViewModel mainViewModel,
            ResultsViewModel results,
            ReviewViewModel review)
        {
            _itunes = itunes;
            _settings = settings;
            _mainViewModel = mainViewModel;
            _results = results;
            _review = review;
        }

        [RelayCommand]
        public void Refresh()
        {
            _selectedTracks = Array.Empty<IITFileOrCDTrack>();
            TrackCount = 0;

            if (!_itunes.TryConnect())
            {
                IsITunesAvailable = false;
                ErrorMessage = DescribeUnavailable();
                return;
            }

            try
            {
                _itunes.EnsureVisible();
                ReadSelection();
                IsITunesAvailable = true;
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                IsITunesAvailable = _itunes.IsAvailable;
                ErrorMessage = "Could not read the iTunes selection: " + ex.Message;
            }
        }

        /// <summary>
        /// Re-reads the selection without side effects, for automatic refreshes (e.g. when the
        /// window is activated). Unlike <see cref="Refresh"/> it never brings iTunes to the front
        /// and never reconnects, since creating the COM object would relaunch iTunes.
        /// </summary>
        public void RefreshSelectionQuietly()
        {
            if (!_itunes.IsAvailable)
                return;

            try
            {
                ReadSelection();
                ErrorMessage = null;
            }
            catch (Exception)
            {
                HandleQuietFailure();
            }
        }

        /// <summary>
        /// Called on a timer while Home is visible. Compares a cheap fingerprint of the live
        /// selection and only re-reads the full track list when it changed.
        /// </summary>
        public void PollSelection()
        {
            if (!_itunes.IsAvailable)
                return;

            try
            {
                if (_itunes.GetSelectionFingerprint() != _lastFingerprint)
                {
                    ReadSelection();
                    ErrorMessage = null;
                }
            }
            catch (Exception)
            {
                HandleQuietFailure();
            }
        }

        private string _lastFingerprint;

        private void ReadSelection()
        {
            _lastFingerprint = _itunes.GetSelectionFingerprint();
            _selectedTracks = _itunes.GetSelectedTracks();
            TrackCount = _selectedTracks.Count;
        }

        private void HandleQuietFailure()
        {
            // A transient failure (iTunes busy with a dialog) keeps the last known selection.
            // If iTunes went away, the service dropped the connection: report that.
            if (_itunes.IsAvailable)
                return;

            _selectedTracks = Array.Empty<IITFileOrCDTrack>();
            _lastFingerprint = null;
            TrackCount = 0;
            IsITunesAvailable = false;
            ErrorMessage = DescribeUnavailable();
        }

        private string DescribeUnavailable()
        {
            const string message = "iTunes is not installed or not responding.";
            return string.IsNullOrWhiteSpace(_itunes.InitError)
                ? message
                : $"{message} ({_itunes.InitError.Trim()})";
        }

        private bool CanGetLyrics() => IsITunesAvailable && TrackCount > 0;

        [RelayCommand(CanExecute = nameof(CanGetLyrics))]
        private void GetLyrics()
        {
            if (string.IsNullOrWhiteSpace(_settings.GeniusApiToken))
            {
                _mainViewModel.Navigate(typeof(SettingsPage));
                return;
            }

            if (IsAutoUpdate)
            {
                _results.Start(_selectedTracks, IsOverwrite);
                _mainViewModel.Navigate(typeof(ResultsPage));
            }
            else
            {
                _review.LoadQueue(_selectedTracks);
                _mainViewModel.Navigate(typeof(ReviewPage));
            }
        }
    }
}
