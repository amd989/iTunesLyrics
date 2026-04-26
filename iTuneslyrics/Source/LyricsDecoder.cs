using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace iTuneslyrics.Source
{
    public class LyricsDecoder
    {
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

        private static readonly HttpClient HttpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            return client;
        }

        private static async Task<string> ReadHtmlAsync(string url)
        {
            try
            {
                return await HttpClient.GetStringAsync(url).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Fetches a Genius lyrics page and extracts the lyrics text.
        /// </summary>
        public static async Task<string> DecodeLyricsAsync(string url)
        {
            try
            {
                var source = await ReadHtmlAsync(url).ConfigureAwait(false);
                if (string.IsNullOrEmpty(source)) return string.Empty;

                var html = new HtmlDocument();
                html.LoadHtml(source);

                // Modern Genius markup: one or more section containers.
                var containers = html.DocumentNode.SelectNodes("//div[@data-lyrics-container='true']");

                // Legacy fallback: very old pages used <div class="lyrics"> wrapping a <p>.
                if (containers == null || containers.Count == 0)
                    containers = html.DocumentNode.SelectNodes("//div[contains(@class,'lyrics')]//p");

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

        /// <summary>
        /// Recursively walks the node tree: text nodes emit decoded text,
        /// &lt;br&gt; emits a newline, block-ish elements emit a trailing newline,
        /// everything else (spans, anchors from annotations) is flattened.
        /// </summary>
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
                        // Genius marks the contributor count, song title, About blurb
                        // and "Read More" button inside the lyrics container with this
                        // attribute so client-side selection skips them. Honor it.
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
    }
}
