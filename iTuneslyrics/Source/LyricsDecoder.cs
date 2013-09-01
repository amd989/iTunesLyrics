using System;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace iTuneslyrics.Source
{
    public class LyricsDecoder
    {
        private static string ReadHtml(string url)
        {
            string result;
            try
            {
                // used to build entire input
                var sb = new StringBuilder();
                
                // used on each read operation
                var buf = new byte[8192];

                // prepare the web page we will be asking for
                var request = (HttpWebRequest) WebRequest.Create(url);

                // execute the request
                var response = (HttpWebResponse) request.GetResponse();

                // we will read data via the response stream
                var stBuffer = response.GetResponseStream();

                int count = 0;
                do
                {
                    // fill the buffer with data
                    count = stBuffer.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count == 0) continue;
                    // translate from bytes to ASCII text
                    string tempString = Encoding.UTF8.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                } while (count > 0); // any more data to read?
                //sb.Replace("<!doctype html>", string.Empty);
                result = sb.ToString();
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Decodes the lyrics from HTML characters to string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DecodeLyrics(string url)
        {
            string result;
            const string divider = "..............";
            var stringBuilder = new StringBuilder();
            try
            {
                var html = new HtmlDocument();
                html.LoadHtml(ReadHtml(url));
                var elements = html.DocumentNode.SelectNodes("//div[@class='lyricbox']");

                foreach (var element in elements)
                {
                    stringBuilder.AppendLine(TransformCodes(element.InnerHtml));
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(divider);
                    stringBuilder.AppendLine();
                }

                var lyrics = stringBuilder.ToString();
                result = lyrics.Remove(lyrics.LastIndexOf(divider, System.StringComparison.Ordinal));
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Transform the codes from HTML to String
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns></returns>
        private static string TransformCodes(string lyrics)
        {
            var result = string.Empty;
            const string comment = "<!--";
            try
            {

                if (!String.IsNullOrEmpty(lyrics))
                {
                    var regex = new Regex("(<div.*div>|<span.*span>)");
                    var toProcess = regex.Replace(lyrics, string.Empty);
                    var index = toProcess.LastIndexOf(comment, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                        toProcess = toProcess.Remove(index);

                    var htmlDecode = HttpUtility.HtmlDecode(toProcess);
                    if (htmlDecode != null)
                        result = htmlDecode.Replace("<br>", Environment.NewLine)
                                           .Replace("<b>", string.Empty)
                                           .Replace("</b>", string.Empty);
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }
    }
}