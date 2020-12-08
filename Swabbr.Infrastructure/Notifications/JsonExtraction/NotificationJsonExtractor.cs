using Swabbr.Core.Types;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Notifications.JsonWrappers;
using System;

namespace Swabbr.Infrastructure.Notifications.JsonExtraction
{
    /// <summary>
    ///     Contains functionality to translate a <see cref="SwabbrNotification"/> to
    ///     the correct JSON template based on the specified platform.
    /// </summary>
    internal static class NotificationJsonExtractor
    {
        /// <summary>
        ///     Extracts a <see cref="PushNotificationPlatform"/> specific template
        ///     for json from a <see cref="SwabbrNotification"/>.
        /// </summary>
        /// <param name="platform"><see cref="PushNotificationPlatform"/></param>
        /// <param name="notification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref="NotificationWrapperJsonBase"/></returns>
        public static NotificationWrapperJsonBase Extract(PushNotificationPlatform platform, SwabbrNotification notification) 
            => platform switch
            {
                PushNotificationPlatform.APNS => FcmJsonExtractor.Extract(notification),
                PushNotificationPlatform.FCM => ApnsJsonExtractor.Extract(notification),
                _ => throw new InvalidOperationException(nameof(platform)),
            };
    }
}
