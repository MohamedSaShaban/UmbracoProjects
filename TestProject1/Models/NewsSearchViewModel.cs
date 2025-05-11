using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestProject1.Models
{
    public class NewsSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public IEnumerable<IPublishedContent> Results { get; set; } = Enumerable.Empty<IPublishedContent>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool IsSearch { get; set; } = false;
        public IEnumerable<IPublishedContent> LatestNews { get; set; } = Enumerable.Empty<IPublishedContent>();
    }

}
