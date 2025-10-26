using System.Collections.Generic;

namespace NewsFetcherAPI.Models
{
    public class GNewsResponse
    {
        public List<Article>? Articles { get; set; }
        public int TotalArticles { get; set; }
    }
}