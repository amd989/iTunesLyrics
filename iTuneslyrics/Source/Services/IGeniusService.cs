using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using iTuneslyrics.Source.Models;

namespace iTuneslyrics.Source.Services
{
    public interface IGeniusService
    {
        Task<IReadOnlyList<SearchHit>> SearchAsync(string query, CancellationToken ct = default);
        Task<string> GetLyricsAsync(string url, CancellationToken ct = default);
    }
}
