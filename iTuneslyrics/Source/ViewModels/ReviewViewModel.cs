using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTunesLib;
using iTuneslyrics.Source.Services;
using iTuneslyrics.Source.Views;

namespace iTuneslyrics.Source.ViewModels
{
    public partial class ReviewViewModel : ObservableObject
    {
        private readonly IGeniusService _genius;
        private readonly IITunesService _itunes;
        private readonly MainViewModel _mainViewModel;
        private IReadOnlyList<IITFileOrCDTrack> _tracks;
        private int _currentIndex;
        private CancellationTokenSource _loadCts;

        [ObservableProperty]
        private string _song;

        [ObservableProperty]
        private string _artist;

        [ObservableProperty]
        private string _lyrics;

        [ObservableProperty]
        private string _albumArtPath;

        [ObservableProperty]
        private string _trackCounter;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _canUpdate = true;

        public ReviewViewModel(IGeniusService genius, IITunesService itunes, MainViewModel mainViewModel)
        {
            _genius = genius;
            _itunes = itunes;
            _mainViewModel = mainViewModel;
        }

        public void LoadQueue(IReadOnlyList<IITFileOrCDTrack> tracks)
        {
            _tracks = tracks;
            _currentIndex = 0;
            LoadCurrent();
        }

        private async void LoadCurrent()
        {
            _loadCts?.Cancel();
            _loadCts = new CancellationTokenSource();
            var ct = _loadCts.Token;

            if (_currentIndex >= _tracks.Count)
            {
                _mainViewModel.Navigate(typeof(HomePage));
                return;
            }

            var track = _tracks[_currentIndex];
            Song = track.Name;
            Artist = track.Artist;
            TrackCounter = $"{_currentIndex + 1} of {_tracks.Count}";
            Lyrics = string.Empty;
            CanUpdate = false;
            IsLoading = true;

            // Album art
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "itunesart");
                _itunes.SaveArtworkToFile(track, tempPath);
                AlbumArtPath = File.Exists(tempPath) ? tempPath : null;
            }
            catch
            {
                AlbumArtPath = null;
            }

            // Fetch lyrics
            try
            {
                var searchArtist = TitleNormalizer.NormalizeForQuery(Artist);
                var searchSong = TitleNormalizer.NormalizeForQuery(Song);
                var hits = await _genius.SearchAsync(searchArtist + " " + searchSong);
                if (ct.IsCancellationRequested) return;

                var match = TitleNormalizer.PickBest(hits, Artist, Song);

                if (match != null)
                {
                    Lyrics = await _genius.GetLyricsAsync(match.Url);
                    if (ct.IsCancellationRequested) return;
                    CanUpdate = !string.IsNullOrEmpty(Lyrics);
                }
                else
                {
                    Lyrics = "No matching lyrics found.";
                    CanUpdate = false;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception)
            {
                if (ct.IsCancellationRequested) return;
                Lyrics = string.Empty;
                CanUpdate = false;
            }
            finally
            {
                if (!ct.IsCancellationRequested)
                    IsLoading = false;
            }
        }

        [RelayCommand]
        private void Update()
        {
            if (_currentIndex < _tracks.Count && !string.IsNullOrEmpty(Lyrics))
            {
                _itunes.SetLyrics(_tracks[_currentIndex], Lyrics);
            }
            _currentIndex++;
            LoadCurrent();
        }

        [RelayCommand]
        private void Skip()
        {
            _currentIndex++;
            LoadCurrent();
        }

        [RelayCommand]
        private void CancelAll()
        {
            _mainViewModel.Navigate(typeof(HomePage));
        }
    }
}
