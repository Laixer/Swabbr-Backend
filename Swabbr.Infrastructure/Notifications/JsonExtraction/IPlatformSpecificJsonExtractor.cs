using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.JsonWrappers;

namespace Swabbr.Infrastructure.Notifications.JsonExtraction
{

    /// <summary>
    /// Contains functionality to extract JSON that is properly foormatted for 
    /// a specific notification service.
    /// </summary>
    internal interface IPlatformSpecificJsonExtractor
    {

        NotificationJsonBase Extract(SwabbrNotification swabbrNotification);

    }

}
