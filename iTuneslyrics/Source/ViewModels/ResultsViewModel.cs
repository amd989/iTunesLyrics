using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using iTunesLib;
using iTuneslyrics.Source.Models;
using iTuneslyrics.Source.Services;

namespace iTuneslyrics.Source.ViewModels
{
    public partial class ResultsViewModel : ObservableObject
    {
        private readonly IGeniusService _genius;
        private readonly IITunesService _itunes;
        private readonly MainViewModel _mainViewModel;
        private CancellationTokenSource _cts;

        public ObservableCollection<TrackResult> Tracks { get; } = new();

        [ObservableProperty]
        private int _progress;

        [ObservableProperty]
        private int _total;

        [ObservableProperty]
        private bool _isProcessing;

        public ResultsViewModel(IGeniusService genius, IITunesService itunes, MainViewModel mainViewModel)
        {
            _genius = genius;
            _itunes = itunes;
            _mainViewModel = mainViewModel;
        }

        public async void Start(IReadOnlyList<IITFileOrCDTrack> tracks, bool overwrite)
        {
            IsProcessing = true;
            Total = tracks.Count;
            Progress = 0;
            _cts = new CancellationTokenSource();

            var updater = new LyricsUpdater(_genius, _itunes);
            var rowMap = new Dictionary<IITFileOrCDTrack, TrackResult>();

            var reporter = new Progress<TrackProgress>(p =>
            {
                if (!rowMap.TryGetValue(p.Track, out var row))
                {
                    row = new TrackResult { Song = p.Song, Artist = p.Artist, Status = p.Status };
                    rowMap[p.Track] = row;
                    Tracks.Add(row);
                }
                else
                {
                    row.Status = p.Status;
                }

                if (p.Status != ResultStatus.Processing)
                    Progress++;
            });

            try
            {
                await updater.UpdateLyricsAsync(tracks, overwrite, reporter, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // cancelled
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
