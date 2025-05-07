using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestProject1.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace TestProject1.Controllers
{
    public class ContactController : SurfaceController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUmbracoDatabase _database;

        public ContactController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
        }

        [HttpGet]
        public IActionResult RenderForm()
        {
            return PartialView("ContactForm", new ContactFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["FormMessage"] = "⚠️ هناك أخطاء في البيانات المدخلة.";
                return CurrentUmbracoPage();
            }

            _database.Insert<ContactFormModel>(model);
            TempData["FormMessage"] = "✅ تم إرسال رسالتك بنجاح، سنقوم بالرد عليك قريبًا.";
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
