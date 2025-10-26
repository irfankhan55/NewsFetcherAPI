using Microsoft.Extensions.Caching.Memory;
using NewsFetcherAPI.Models;
using System.Net.Http.Json;

namespace NewsFetcherAPI.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _apiKey = "e01ac10cdd55771632a98efa8014e964"; // Replace this
        private readonly string _baseUrl = "https://gnews.io/api/v4/";

         public NewsService(IMemoryCache cache)
        {
            _httpClient = new HttpClient();
            _cache = cache;
        }

        public async Task<IEnumerable<Article>> GetLatestNewsAsync(int count)
        {
            string cacheKey = $"latest_{count}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Article> cachedNews))
                return cachedNews;

            var url = $"{_baseUrl}top-headlines?max={count}&lang=en&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<GNewsResponse>(url)
                           ?? new GNewsResponse { Articles = new List<Article>() };

            var articles = response.Articles ?? new List<Article>();
            _cache.Set(cacheKey, articles, TimeSpan.FromMinutes(10));
            return articles;
        }

        public async Task<Article?> GetArticleByTitleOrAuthorAsync(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return null;

            query = query.Trim();
            string cacheKey = $"article_{query}";
            if (_cache.TryGetValue(cacheKey, out Article cachedArticle))
                return cachedArticle;

            // Use title-specific search in API
            var titleUrl = $"{_baseUrl}search?q={Uri.EscapeDataString(query)}&in=title&lang=en&token={_apiKey}";
            var titleResponse = await _httpClient.GetFromJsonAsync<GNewsResponse>(titleUrl);
            var titleArticles = titleResponse?.Articles ?? new List<Article>();

            // Fetch by keyword to cover author search
            var keywordArticles = await SearchByKeywordAsync(query);

            // Combine results
            var allArticles = titleArticles.Concat(keywordArticles)
                                        .DistinctBy(a => a.Title) // prevent duplicates
                                        .ToList();

            // Find first match by title OR author
            var result = allArticles.FirstOrDefault(a =>
                (!string.IsNullOrEmpty(a.Title) && a.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(a.Author) && a.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
            );

            if (result != null)
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<IEnumerable<Article>> SearchByKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Article>();

            keyword = keyword.Trim();
            string cacheKey = $"keyword_{keyword}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Article> cachedResults))
                return cachedResults;

            var url = $"{_baseUrl}search?q={Uri.EscapeDataString(keyword)}&lang=en&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<GNewsResponse>(url);

            var articles = response?.Articles ?? new List<Article>();
            _cache.Set(cacheKey, articles, TimeSpan.FromMinutes(10));

            return articles;
        }
    }
}