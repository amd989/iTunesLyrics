using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using iTuneslyrics.Source.Models;

namespace iTuneslyrics.Source.Services
{
    internal static class TitleNormalizer
    {
        public const int AcceptanceThreshold = 65;

        private static readonly Regex FeaturingClause = new Regex(
            @"\s*[\(\[]\s*(feat\.?|featuring|ft\.?|with)\s[^)\]]*[\)\]]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex EditionClauseParen = new Regex(
            @"\s*[\(\[]\s*(remastered|remaster|re\-?recorded|live|acoustic|unplugged|radio edit|single version|album version|extended( mix)?|deluxe|bonus track|mono|stereo|explicit|clean|instrumental|demo|edit|remix|version)\b[^)\]]*[\)\]]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex EditionClauseDash = new Regex(
            @"\s*[-\u2013\u2014]\s*(remastered|remaster|re\-?recorded|live|acoustic|unplugged|radio edit|single version|album version|extended( mix)?|deluxe|bonus track|mono|stereo|explicit|clean|instrumental|demo|edit|remix|version)\b.*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Punctuation = new Regex(@"[^\w\s]", RegexOptions.Compiled);
        private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        public static string NormalizeForQuery(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var stripped = FeaturingClause.Replace(value, " ");
            stripped = EditionClauseParen.Replace(stripped, " ");
            stripped = EditionClauseDash.Replace(stripped, " ");
            return Whitespace.Replace(stripped, " ").Trim();
        }

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
}
