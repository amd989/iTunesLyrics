using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FuzzySharp;
using iTunesLib;
using iTuneslyrics.Properties;

namespace iTuneslyrics.Source
{
    internal static class TitleNormalizer
    {
        // Minimum weighted score (title 0.7 + artist 0.3) required to accept a Genius
        // hit. Tuned empirically — above ~65 on TokenSetRatio usually means the same
        // song modulo remaster/edition tags, below it is typically a different track.
        public const int AcceptanceThreshold = 65;

        // "(feat. X)", "(ft X)", "(featuring X)", "(with X)" — iTunes embeds
        // featurings in the title, Genius keeps them out-of-band.
        private static readonly Regex FeaturingClause = new Regex(
            @"\s*[\(\[]\s*(feat\.?|featuring|ft\.?|with)\s[^)\]]*[\)\]]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // "(Remastered 2009)", "[Deluxe Edition]", "(Live at Wembley)" etc.
        private static readonly Regex EditionClauseParen = new Regex(
            @"\s*[\(\[]\s*(remastered|remaster|re\-?recorded|live|acoustic|unplugged|radio edit|single version|album version|extended( mix)?|deluxe|bonus track|mono|stereo|explicit|clean|instrumental|demo|edit|remix|version)\b[^)\]]*[\)\]]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Trailing " - Remastered 2009", " - Live", etc. (no parens — common on Spotify imports).
        private static readonly Regex EditionClauseDash = new Regex(
            @"\s*[-\u2013\u2014]\s*(remastered|remaster|re\-?recorded|live|acoustic|unplugged|radio edit|single version|album version|extended( mix)?|deluxe|bonus track|mono|stereo|explicit|clean|instrumental|demo|edit|remix|version)\b.*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Punctuation = new Regex(@"[^\w\s]", RegexOptions.Compiled);
        private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        // Kept separate from Normalize: the Genius search query benefits from featuring
        // + edition stripping, but not from aggressive diacritic/punctuation stripping
        // (Genius handles those fine and stripping them hurts recall for non-Latin artists).
        public static string NormalizeForQuery(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var stripped = FeaturingClause.Replace(value, " ");
            stripped = EditionClauseParen.Replace(stripped, " ");
            stripped = EditionClauseDash.Replace(stripped, " ");
            return Whitespace.Replace(stripped, " ").Trim();
        }

        // Full normalization for fuzzy comparison: strips featurings, edition tags,
        // diacritics, punctuation, and lowercases.
        public static string Normalize(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var stripped = NormalizeForQuery(value);
            var folded = StripDiacritics(stripped);
            var noPunct = Punctuation.Replace(folded, " ");
            return Whitespace.Replace(noPunct, " ").Trim().ToLowerInvariant();
        }

        private static string StripDiacritics(string value)
        {
            var decomposed = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static int Score(SearchHit hit, string artist, string song)
        {
            if (hit == null) return 0;
            var titleScore = Fuzz.TokenSetRatio(Normalize(hit.Title), Normalize(song));
            var artistScore = Fuzz.TokenSetRatio(Normalize(hit.ArtistName), Normalize(artist));
            return (int)Math.Round(titleScore * 0.7 + artistScore * 0.3);
        }

        public static SearchHit PickBest(IEnumerable<SearchHit> hits, string artist, string song, int threshold = AcceptanceThreshold)
        {
            if (hits == null) return null;
            SearchHit best = null;
            var bestScore = -1;
            foreach (var h in hits)
            {
                var s = Score(h, artist, song);
                if (s > bestScore)
                {
                    bestScore = s;
                    best = h;
                }
            }
            return bestScore >= threshold ? best : null;
        }
    }

    class LyricsUpdater
    {
        private readonly List<IITFileOrCDTrack> mSelectedTracks;
        private readonly IGeniusService geniusService;
        private readonly frmResult mForm;
        private readonly bool mOverwrite;

        public LyricsUpdater(List<IITFileOrCDTrack> selectedTracks, IGeniusService geniusService, bool overwrite, frmResult form)
        {
            this.mSelectedTracks = selectedTracks;
            this.geniusService = geniusService;
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
                    var searchArtist = TitleNormalizer.NormalizeForQuery(artist);
                    var searchSong = TitleNormalizer.NormalizeForQuery(song);
                    var hits = await this.geniusService.SearchAsync(searchArtist + " " + searchSong);
                    var hit = TitleNormalizer.PickBest(hits, artist, song);
                    if (hit == null)
                    {
                        this.mForm.UpdateRow(index, ResultCodes.NotFound);
                        continue;
                    }

                    if (this.mOverwrite || currentTrack.Lyrics == null)
                    {
                        await SetLyricsAsync(currentTrack, hit.Url, index);
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
            var lyrics = await geniusService.GetLyricsAsync(lyricsUrl);
            var isFound = string.IsNullOrEmpty(lyrics) ? ResultCodes.NotFound : ResultCodes.Found;

            if (isFound == ResultCodes.Found)
                currentTrack.Lyrics = lyrics;

            this.mForm.UpdateRow(index, isFound);
        }
    }
}
