using System.Collections.Generic;
using iTunesLib;

namespace iTuneslyrics.Source.Services
{
    public interface IITunesService
    {
        bool IsAvailable { get; }
        IReadOnlyList<IITFileOrCDTrack> GetSelectedTracks();
        void SetLyrics(IITFileOrCDTrack track, string lyrics);
        void SaveArtworkToFile(IITFileOrCDTrack track, string path);
        void EnsureVisible();
    }
}
