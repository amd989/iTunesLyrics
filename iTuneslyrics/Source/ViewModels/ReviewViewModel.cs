using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private IReadOnlyList<IITFileOrCDTrack> _tracks = Array.Empty<IITFileOrCDTrack>();
        private int _currentIndex;
        private CancellationTokenSource _loadCts;

        [ObservableProperty]
        private string _song;

        [ObservableProperty]
        private string _artist;

        [ObservableProperty]
        private string _lyrics;

        [ObservableProperty]
        private ImageSource _albumArt;

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
            CancelPendingLoad();
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
            AlbumArt = LoadArtwork(track);

            try
            {
                var searchArtist = TitleNormalizer.NormalizeForQuery(Artist);
                var searchSong = TitleNormalizer.NormalizeForQuery(Song);
                var hits = await _genius.SearchAsync(searchArtist + " " + searchSong, ct);
                if (ct.IsCancellationRequested) return;

                var match = TitleNormalizer.PickBest(hits, Artist, Song);

                if (match != null)
                {
                    Lyrics = await _genius.GetLyricsAsync(match.Url, ct);
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

        /// <summary>
        /// Exports the track's artwork to a unique temp file, decodes it fully into memory,
        /// then deletes the file. Avoids WPF's URI image cache and leftover temp files.
        /// </summary>
        private ImageSource LoadArtwork(IITFileOrCDTrack track)
        {
            var path = Path.Combine(Path.GetTempPath(), $"itunesart_{Guid.NewGuid():N}");
            try
            {
                _itunes.SaveArtworkToFile(track, path);
                if (!File.Exists(path))
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.DecodePixelWidth = 128;
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
            finally
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch
                {
                    // best effort
                }
            }
        }

        private void CancelPendingLoad()
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;
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
            CancelPendingLoad();
            _tracks = Array.Empty<IITFileOrCDTrack>();
            _mainViewModel.Navigate(typeof(HomePage));
        }
    }
}
