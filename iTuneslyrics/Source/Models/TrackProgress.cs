using iTunesLib;

namespace iTuneslyrics.Source.Models
{
    public record TrackProgress(
        IITFileOrCDTrack Track,
        string Song,
        string Artist,
        ResultStatus Status,
        string Lyrics = null);
}
