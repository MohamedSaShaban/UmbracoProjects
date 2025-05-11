using TestProject1.Validations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;

namespace TestProject1.Composers
{
    public class NewsValidationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentSavingNotification, NewsItemValidationHandler>();
        }
    }

}
