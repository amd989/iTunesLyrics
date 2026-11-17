using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTunesLib;
using iTuneslyrics.Source.Models;
using iTuneslyrics.Source.Services;

namespace iTuneslyrics.Source.ViewModels
{
    /// <summary>
    /// Holds the most recent auto-update run. Registered as a singleton so the
    /// run survives navigating away from the Results page and back.
    /// </summary>
    public partial class ResultsViewModel : ObservableObject
    {
        private readonly IGeniusService _genius;
        private readonly IITunesService _itunes;
        private CancellationTokenSource _cts;

        public ObservableCollection<TrackResult> Tracks { get; } = new();

        [ObservableProperty]
        private int _progress;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasResults))]
        private int _total;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isProcessing;

        [ObservableProperty]
        private string _statusText = "No results yet";

        [ObservableProperty]
        private TrackResult _currentTrack;

        public bool HasResults => Total > 0;

        public ResultsViewModel(IGeniusService genius, IITunesService itunes)
        {
            _genius = genius;
            _itunes = itunes;
        }

        public async void Start(IReadOnlyList<IITFileOrCDTrack> tracks, bool overwrite)
        {
            // Supersede any run still in progress.
            _cts?.Cancel();
            var cts = new CancellationTokenSource();
            _cts = cts;

            Tracks.Clear();
            CurrentTrack = null;
            Total = tracks.Count;
            Progress = 0;
            IsProcessing = true;
            StatusText = "Processing";

            var updater = new LyricsUpdater(_genius, _itunes);
            var rowMap = new Dictionary<IITFileOrCDTrack, TrackResult>();

            var reporter = new Progress<TrackProgress>(p =>
            {
                // Reports are posted asynchronously; drop any that arrive after this run was cancelled or replaced.
                if (cts.IsCancellationRequested)
                    return;

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

                CurrentTrack = row;

                if (p.Status != ResultStatus.Processing)
                    Progress++;
            });

            string failure = null;
            try
            {
                await updater.UpdateLyricsAsync(tracks, overwrite, reporter, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // cancelled or superseded
            }
            catch (Exception ex)
            {
                failure = ex.Message;
            }
            finally
            {
                if (ReferenceEquals(_cts, cts))
                {
                    _cts = null;
                    IsProcessing = false;

                    if (cts.IsCancellationRequested)
                    {
                        StatusText = "Cancelled";
                        foreach (var row in Tracks)
                        {
                            if (row.Status == ResultStatus.Processing)
                                row.Status = ResultStatus.Skipped;
                        }
                    }
                    else
                    {
                        StatusText = failure == null ? "Done" : "Stopped: " + failure;
                    }
                }
                cts.Dispose();
            }
        }

        [RelayCommand(CanExecute = nameof(IsProcessing))]
        private void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
