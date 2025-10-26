using NewsFetcherAPI.Models;

namespace NewsFetcherAPI.Services
{
    public interface INewsService
    {
        Task<IEnumerable<Article>> GetLatestNewsAsync(int count);
        Task<Article?> GetArticleByTitleOrAuthorAsync(string query);
    }
}