using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genius;
using Genius.Core;
using iTunesLib;
using iTuneslyrics.Properties;

namespace iTuneslyrics.Source
{
    internal static class TitleNormalizer
    {
        // Strips "(feat. X)", "(ft X)", "(featuring X)", "(with X)" and the
        // bracketed equivalents. iTunes metadata embeds featurings in the song
        // title, Genius keeps them out-of-band — normalize both sides before
        // comparing.
        private static readonly Regex FeaturingClause = new Regex(
            @"\s*[\(\[]\s*(feat\.?|featuring|ft\.?|with)\s[^)\]]*[\)\]]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        public static string Normalize(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var stripped = FeaturingClause.Replace(value, " ");
            return Whitespace.Replace(stripped, " ").Trim();
        }

        public static bool Matches(string a, string b)
        {
            return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
        }
    }

    class LyricsUpdater
    {
        private readonly List<IITFileOrCDTrack> mSelectedTracks;
        private readonly GeniusClient geniusClient;
        private readonly frmResult mForm;
        private readonly bool mOverwrite;

        public LyricsUpdater(List<IITFileOrCDTrack> selectedTracks, IGeniusClient geniusClient, bool overwrite, frmResult form)
        {
            this.mSelectedTracks = selectedTracks;
            this.geniusClient = (GeniusClient)geniusClient;
            this.mOverwrite = overwrite;
            this.mForm = form;
        }

        public async Task UpdateLyricsAsync()
        {
            foreach (var currentTrack in this.mSelectedTracks)
            {
                var artist = currentTrack.Artist;
                var song = currentTrack.Name;

                if (string.IsNullOrEmpty(currentTrack.Location) || string.IsNullOrEmpty(artist) ||
                    string.IsNullOrEmpty(song)) continue;

                var index = this.mForm.AddRow(new[] { song, artist, "Processing..." });

                if (currentTrack.Lyrics != null && !this.mOverwrite)
                {
                    this.mForm.UpdateRow(index, ResultCodes.Skipped);
                    continue;
                }

                try
                {
                    var searchArtist = TitleNormalizer.Normalize(artist);
                    var searchSong = TitleNormalizer.Normalize(song);
                    var query = await this.geniusClient.SearchClient.Search(searchArtist + " " + searchSong);
                    var hit = query.Response.Hits.FirstOrDefault();
                    if (hit == null)
                    {
                        this.mForm.UpdateRow(index, ResultCodes.NotFound);
                        continue;
                    }

                    if (this.mOverwrite || currentTrack.Lyrics == null)
                    {
                        await SetLyricsAsync(currentTrack, hit.Result.Url, index);
                    }
                    else
                    {
                        this.mForm.UpdateRow(index, ResultCodes.Skipped);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    this.mForm.UpdateRow(index, ResultCodes.NotFound);
                }
            }
            MessageBox.Show(Resources.LyricsUpdater_UpdateLyrics_Completed, Resources.LyricsUpdater_UpdateLyrics_Completed, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task SetLyricsAsync(IITFileOrCDTrack currentTrack, string lyricsUrl, int index)
        {
            var lyrics = await LyricsDecoder.DecodeLyricsAsync(lyricsUrl);
            var isFound = string.IsNullOrEmpty(lyrics) ? ResultCodes.NotFound : ResultCodes.Found;

            if (isFound == ResultCodes.Found)
                currentTrack.Lyrics = lyrics;

            this.mForm.UpdateRow(index, isFound);
        }
    }
}
