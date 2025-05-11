using Microsoft.AspNetCore.Mvc;
using TestProject1.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Examine;
using Examine.Search;
using Umbraco.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestProject1.Controllers
{
    public class NewsController : SurfaceController
    {
        private readonly IExamineManager _examineManager;
        private readonly IPublishedContentQuery _contentQuery;

        public NewsController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IExamineManager examineManager,
            IPublishedContentQuery contentQuery)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _examineManager = examineManager;
            _contentQuery = contentQuery;
        }

        [HttpGet]
        public IActionResult Search(string searchTerm, int page = 1)
        {
            var rootContent = _contentQuery.ContentAtRoot().FirstOrDefault();

            var newsPage = rootContent?.Children.FirstOrDefault(e => e.Name == "News");
                //.DescendantsOrSelf("newsListPage")
                //.FirstOrDefault();

            var pageSize = newsPage?.Value<int>("pageSize") > 0 ? newsPage.Value<int>("pageSize") : 5;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ViewBag.Message = "يرجى إدخال كلمة للبحث.";
                ViewBag.NewsModel = new NewsSearchViewModel
                {
                    SearchTerm = searchTerm,
                    Results = Enumerable.Empty<IPublishedContent>(),
                    CurrentPage = 1,
                    TotalPages = 0,
                    IsSearch = true
                };
                return View("NewsListPage");
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out var index))
                return NotFound();

            var searcher = index.Searcher;

            var criteria = searcher.CreateQuery("content")
                                   .NodeTypeAlias("newsItem")
                                   .And()
                                   .GroupedOr(new[] { "newsTitle", "newsBody" }, searchTerm);

            var results = criteria.Execute();
            var totalResults = results.TotalItemCount;

            var pagedResults = results
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => int.TryParse(r.Id, out var id) ? UmbracoContext?.Content?.GetById(id) : null)
                .Where(c => c != null);

            var model = new NewsSearchViewModel
            {
                SearchTerm = searchTerm,
                Results = pagedResults,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalResults / pageSize),
                IsSearch = true
            };

            ViewBag.NewsModel = model;

            return View("NewsListPage");
        }


    }
}
