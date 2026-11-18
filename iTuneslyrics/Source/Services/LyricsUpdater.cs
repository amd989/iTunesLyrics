using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using iTunesLib;
using iTuneslyrics.Source.Models;

namespace iTuneslyrics.Source.Services
{
    public class LyricsUpdater
    {
        private readonly IGeniusService _genius;
        private readonly IITunesService _itunes;

        public LyricsUpdater(IGeniusService genius, IITunesService itunes)
        {
            _genius = genius;
            _itunes = itunes;
        }

        public async Task UpdateLyricsAsync(
            IReadOnlyList<IITFileOrCDTrack> tracks,
            bool overwrite,
            IProgress<TrackProgress> progress,
            CancellationToken ct)
        {
            foreach (var track in tracks)
            {
                ct.ThrowIfCancellationRequested();

                var artist = track.Artist;
                var song = track.Name;

                if (string.IsNullOrEmpty(track.Location) ||
                    string.IsNullOrEmpty(artist) ||
                    string.IsNullOrEmpty(song))
                {
                    progress.Report(new TrackProgress(track, song ?? string.Empty, artist ?? string.Empty, ResultStatus.Skipped));
                    continue;
                }

                progress.Report(new TrackProgress(track, song, artist, ResultStatus.Processing));

                if (track.Lyrics != null && !overwrite)
                {
                    progress.Report(new TrackProgress(track, song, artist, ResultStatus.Skipped));
                    continue;
                }

                try
                {
                    var searchArtist = TitleNormalizer.NormalizeForQuery(artist);
                    var searchSong = TitleNormalizer.NormalizeForQuery(song);
                    var hits = await _genius.SearchAsync(searchArtist + " " + searchSong, ct);
                    var hit = TitleNormalizer.PickBest(hits, artist, song);

                    if (hit == null)
                    {
                        progress.Report(new TrackProgress(track, song, artist, ResultStatus.NotFound));
                        continue;
                    }

                    var lyrics = await _genius.GetLyricsAsync(hit.Url, ct);
                    if (string.IsNullOrEmpty(lyrics))
                    {
                        progress.Report(new TrackProgress(track, song, artist, ResultStatus.NotFound));
                        continue;
                    }

                    _itunes.SetLyrics(track, lyrics);
                    progress.Report(new TrackProgress(track, song, artist, ResultStatus.Found));
                }
                catch (Exception)
                {
                    progress.Report(new TrackProgress(track, song, artist, ResultStatus.Error));
                }
            }
        }
    }
}
