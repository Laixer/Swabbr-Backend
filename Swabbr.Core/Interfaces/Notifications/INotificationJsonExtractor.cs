using Swabbr.Core.Enums;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.JsonWrappers;

namespace Swabbr.Core.Interfaces.Notifications
{

    /// <summary>
    /// Contains functionality to generate platform specific JSON template objects
    /// based on a <see cref="SwabbrNotification"/>. We need this  because each 
    /// <see cref="PushNotificationPlatform"/> sends its data payload in a different 
    /// way.
    /// </summary>
    public interface INotificationJsonExtractor
    {

        NotificationJsonBase Extract(PushNotificationPlatform platform, SwabbrNotification notification);

    }
}
