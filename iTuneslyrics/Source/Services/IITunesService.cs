using System.Collections.Generic;
using iTunesLib;

namespace iTuneslyrics.Source.Services
{
    public interface IITunesService
    {
        bool IsAvailable { get; }

        /// <summary>Reason the last connection attempt failed, or null.</summary>
        string InitError { get; }

        /// <summary>Connects to iTunes if not already connected. Returns IsAvailable.</summary>
        bool TryConnect();

        IReadOnlyList<IITFileOrCDTrack> GetSelectedTracks();

        /// <summary>
        /// Cheap identity of the current selection (count plus first/last track IDs), for polling.
        /// Changes whenever the selection does, in all but contrived cases. Empty when not connected.
        /// </summary>
        string GetSelectionFingerprint();
        void SetLyrics(IITFileOrCDTrack track, string lyrics);
        void SaveArtworkToFile(IITFileOrCDTrack track, string path);
        void EnsureVisible();
    }
}
