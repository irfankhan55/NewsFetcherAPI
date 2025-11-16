using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NewsFetcherAPI.Configurators;
using NewsFetcherAPI.Models;
using NewsFetcherAPI.Services;
using Xunit;

namespace NewsFetcherAPI.UnitTests
{
    public class NewsServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<GNewsSettings> _options;
        private readonly NewsService _service;

        public NewsServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var settings = new GNewsSettings
            {
                BaseUrl = "https://mockapi.com/",
                ApiKey = "dummy_api_key"
            };
            _options = Options.Create(settings);
            _service = new NewsService(_memoryCache, _options);
        }

        [Fact]
        public async Task GetLatestNewsAsync_ReturnsArticles_WhenCacheEmpty()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Title1", Author = "Author1" },
                new Article { Title = "Title2", Author = "Author2" }
            };

            var httpClientField = typeof(NewsService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField!.SetValue(_service, new HttpClient(new FakeHttpHandler(articles)));
            var result = await _service.GetLatestNewsAsync(2);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetArticleByTitleOrAuthorAsync_ReturnsNull_WhenQueryNullOrWhitespace()
        {
            var result1 = await _service.GetArticleByTitleOrAuthorAsync(null);
            var result2 = await _service.GetArticleByTitleOrAuthorAsync(" ");

            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task SearchByKeywordAsync_ReturnsEmpty_WhenKeywordNullOrWhitespace()
        {
            var result1 = await _service.SearchByKeywordAsync(null!);
            var result2 = await _service.SearchByKeywordAsync("   ");

            Assert.Empty(result1);
            Assert.Empty(result2);
        }

        [Fact]
        public async Task GetArticleByTitleOrAuthorAsync_ReturnsArticle_WhenTitleMatches()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Breaking News", Author = "John Doe" }
            };

            var httpClientField = typeof(NewsService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField!.SetValue(_service, new HttpClient(new FakeHttpHandler(articles)));
            var result = await _service.GetArticleByTitleOrAuthorAsync("Breaking");
            Assert.NotNull(result);
            Assert.Equal("Breaking News", result!.Title);
        }
        private class FakeHttpHandler : HttpMessageHandler
        {
            private readonly List<Article> _articles;
            public FakeHttpHandler(List<Article> articles)
            {
                _articles = articles;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new GNewsResponse { Articles = _articles };
                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(response)
                };
                return Task.FromResult(message);
            }
        }
    }
}