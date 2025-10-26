using Microsoft.AspNetCore.Mvc;

namespace NewsFetcherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("API is running ✅");
        }
    }
}