using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace TestProject1.Validations
{
    public class NewsItemValidationHandler : INotificationHandler<ContentSavingNotification>
    {
        public void Handle(ContentSavingNotification notification)
        {
            foreach (var content in notification.SavedEntities)
            {
                if (content.ContentType.Alias == "newsItem")
                {
                    var title = content.GetValue<string>("newsTitle");
                    var summary = content.GetValue<string>("newsBody");
                    var publishDate = content.GetValue<DateTime>("publishDate");

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        notification.CancelOperation(new EventMessage("Validation", "العنوان مطلوب", EventMessageType.Error));
                    }

                    if (string.IsNullOrWhiteSpace(summary))
                    {
                        notification.CancelOperation(new EventMessage("Validation", "الملخص مطلوب", EventMessageType.Error));
                    }

                    if (publishDate > DateTime.Now.Date)
                    {
                        notification.CancelOperation(new EventMessage("Validation", "لا يمكن ان يكون تاريخ النشر اكبر من تاريخ اليوم", EventMessageType.Error));
                    }
                }
            }
        }
    }
}
