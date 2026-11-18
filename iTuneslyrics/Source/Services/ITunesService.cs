using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using iTunesLib;

namespace iTuneslyrics.Source.Services
{
    public class ITunesService : IITunesService
    {
        private IiTunes _app;

        public string InitError { get; private set; }

        public ITunesService()
        {
            TryConnect();
        }

        public bool IsAvailable => _app != null;

        public bool TryConnect()
        {
            if (_app != null)
                return true;

            try
            {
                _app = new iTunesApp();
                InitError = null;
            }
            catch (Exception ex)
            {
                _app = null;
                InitError = ex.Message;
            }

            return IsAvailable;
        }

        public IReadOnlyList<IITFileOrCDTrack> GetSelectedTracks()
        {
            if (_app == null)
                return Array.Empty<IITFileOrCDTrack>();

            return WithDisconnectHandling(() =>
            {
                var selected = _app.SelectedTracks;
                if (selected == null)
                    return (IReadOnlyList<IITFileOrCDTrack>)Array.Empty<IITFileOrCDTrack>();

                return selected.OfType<IITFileOrCDTrack>().ToList();
            });
        }

        public string GetSelectionFingerprint()
        {
            if (_app == null)
                return string.Empty;

            return WithDisconnectHandling(() =>
            {
                var selected = _app.SelectedTracks;
                if (selected == null || selected.Count == 0)
                    return "0";

                // Count plus first/last database IDs: O(1) COM calls regardless of selection size.
                var count = selected.Count;
                return $"{count}:{selected[1].TrackDatabaseID}:{selected[count].TrackDatabaseID}";
            });
        }

        private T WithDisconnectHandling<T>(Func<T> call)
        {
            try
            {
                return call();
            }
            catch (COMException ex) when (IsDisconnected(ex))
            {
                // iTunes was closed; drop the stale proxy so the next TryConnect reconnects.
                _app = null;
                InitError = ex.Message;
                throw;
            }
        }

        // RPC_E_DISCONNECTED, RPC_S_SERVER_UNAVAILABLE, CO_E_OBJNOTCONNECTED. Anything else
        // (e.g. RPC_E_CALL_REJECTED while iTunes shows a dialog) is transient: keep the connection.
        private static bool IsDisconnected(COMException ex) =>
            ex.HResult is unchecked((int)0x80010108) or unchecked((int)0x800706BA) or unchecked((int)0x800401FD);

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
