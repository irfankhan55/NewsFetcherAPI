using Microsoft.AspNetCore.Mvc;
using NewsFetcherAPI.Services;

namespace NewsFetcherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestNews([FromQuery] int count = 5)
        {
            var news = await _newsService.GetLatestNewsAsync(count);
            return Ok(news);
        }

        [HttpGet("find")]
        public async Task<IActionResult> FindArticle([FromQuery] string query)
        {
            var article = await _newsService.GetArticleByTitleOrAuthorAsync(query);
            if (article == null)
                return NotFound("No matching article found.");
            return Ok(article);
        }
    }
}