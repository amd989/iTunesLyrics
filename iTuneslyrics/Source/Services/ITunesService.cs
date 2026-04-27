using System;
using System.Collections.Generic;
using System.Linq;
using iTunesLib;

namespace iTuneslyrics.Source.Services
{
    public class ITunesService : IITunesService
    {
        private readonly IiTunes _app;

        public ITunesService()
        {
            try
            {
                _app = new iTunesApp();
            }
            catch
            {
                _app = null;
            }
        }

        public bool IsAvailable => _app != null;

        public IReadOnlyList<IITFileOrCDTrack> GetSelectedTracks()
        {
            if (_app?.SelectedTracks == null)
                return Array.Empty<IITFileOrCDTrack>();

            return _app.SelectedTracks.OfType<IITFileOrCDTrack>().ToList();
        }

        public void SetLyrics(IITFileOrCDTrack track, string lyrics)
        {
            track.Lyrics = lyrics;
        }

        public void SaveArtworkToFile(IITFileOrCDTrack track, string path)
        {
            if (track.Artwork != null && track.Artwork.Count > 0)
                track.Artwork[1].SaveArtworkToFile(path);
        }

        public void EnsureVisible()
        {
            if (_app == null) return;
            try
            {
                _app.BrowserWindow.Visible = true;
                _app.BrowserWindow.Minimized = false;
            }
            catch
            {
                // iTunes may not be responding
            }
        }
    }
}
