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
    }
}