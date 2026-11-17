using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using iTuneslyrics.Source.Models;

namespace iTuneslyrics.Source.Services
{
    public class GeniusService : IGeniusService
    {
        private const string SearchEndpoint = "https://api.genius.com/search";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly HttpClient _httpClient;
        private readonly IUserSettings _settings;

        public GeniusService(HttpClient httpClient, IUserSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<IReadOnlyList<SearchHit>> SearchAsync(string query, CancellationToken ct = default)
        {
            try
            {
                var token = _settings.GeniusApiToken;
                if (string.IsNullOrWhiteSpace(token))
                    return Array.Empty<SearchHit>();

                var url = $"{SearchEndpoint}?q={Uri.EscapeDataString(query)}";
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var searchResponse = JsonSerializer.Deserialize<GeniusSearchResponse>(json, JsonOptions);

                var hits = searchResponse?.Response?.Hits;
                if (hits == null || hits.Count == 0)
                    return Array.Empty<SearchHit>();

                var results = new List<SearchHit>(hits.Count);
                foreach (var h in hits)
                {
                    if (h.Result == null) continue;
                    results.Add(new SearchHit
                    {
                        Title = h.Result.Title,
                        Url = h.Result.Url,
                        ArtistName = h.Result.PrimaryArtist?.Name
                    });
                }
                return results;
            }
            catch (Exception)
            {
                return Array.Empty<SearchHit>();
            }
        }

        public async Task<string> GetLyricsAsync(string url, CancellationToken ct = default)
        {
            try
            {
                var source = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);
                if (string.IsNullOrEmpty(source)) return string.Empty;

                var html = new HtmlDocument();
                html.LoadHtml(source);

                var containers = html.DocumentNode.SelectNodes(
                    "//div[@data-lyrics-container='true']");

                if (containers == null || containers.Count == 0)
                    containers = html.DocumentNode.SelectNodes(
                        "//div[contains(@class,'lyrics')]//p");

                if (containers == null || containers.Count == 0) return string.Empty;

                var sb = new StringBuilder();
                foreach (var node in containers)
                {
                    AppendNodeText(node, sb);
                    if (sb.Length > 0 && sb[sb.Length - 1] != '\n')
                        sb.Append('\n');
                }

                return CollapseBlankLines(sb.ToString()).Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static void AppendNodeText(HtmlNode node, StringBuilder sb)
        {
            foreach (var child in node.ChildNodes)
            {
                switch (child.NodeType)
                {
                    case HtmlNodeType.Text:
                        sb.Append(WebUtility.HtmlDecode(((HtmlTextNode)child).Text));
                        break;

                    case HtmlNodeType.Element:
                        if (string.Equals(
                                child.GetAttributeValue("data-exclude-from-selection", ""),
                                "true",
                                StringComparison.OrdinalIgnoreCase))
                            break;

                        var name = child.Name.ToLowerInvariant();
                        if (name == "br")
                        {
                            sb.Append('\n');
                        }
                        else if (name == "script" || name == "style")
                        {
                            // skip
                        }
                        else
                        {
                            AppendNodeText(child, sb);
                            if (IsBlockElement(name) && sb.Length > 0 && sb[sb.Length - 1] != '\n')
                                sb.Append('\n');
                        }
                        break;
                }
            }
        }

        private static bool IsBlockElement(string name)
        {
            switch (name)
            {
                case "p":
                case "div":
                case "li":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                    return true;
                default:
                    return false;
            }
        }

        private static string CollapseBlankLines(string input)
        {
            var lines = input.Replace("\r\n", "\n").Split('\n');
            var sb = new StringBuilder(input.Length);
            var blankRun = 0;
            foreach (var raw in lines)
            {
                var line = raw.TrimEnd();
                if (line.Length == 0)
                {
                    if (++blankRun > 1) continue;
                }
                else
                {
                    blankRun = 0;
                }
                sb.Append(line).Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        #region JSON Models (Genius API response)

        private class GeniusSearchResponse
        {
            public GeniusResponseBody Response { get; set; }
        }

        private class GeniusResponseBody
        {
            public List<GeniusHit> Hits { get; set; }
        }

        private class GeniusHit
        {
            public GeniusSong Result { get; set; }
        }

        private class GeniusSong
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public GeniusArtist PrimaryArtist { get; set; }
        }

        private class GeniusArtist
        {
            public string Name { get; set; }
        }

        #endregion
    }
}
