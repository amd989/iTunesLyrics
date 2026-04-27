using System.Collections.Generic;
using System.Threading.Tasks;

namespace iTuneslyrics.Source.Services
{
    public interface IGeniusService
    {
        Task<IReadOnlyList<SearchHit>> SearchAsync(string query);
        Task<string> GetLyricsAsync(string url);
    }
}
