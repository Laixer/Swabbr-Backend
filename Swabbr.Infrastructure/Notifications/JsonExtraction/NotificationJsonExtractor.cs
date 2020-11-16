using Swabbr.Core.Enums;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.JsonExtraction;
using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Infrastructure.Notifications.JsonExtraction
{
    /// <summary>
    ///     Contains functionality to translate a <see cref="SwabbrNotification"/> to
    ///     the correct JSON template based on the specified platform.
    /// </summary>
    public static class NotificationJsonExtractor
    {
        /// <summary>
        ///     Extracts a <see cref="PushNotificationPlatform"/> specific template
        ///     for json from a <see cref="SwabbrNotification"/>.
        /// </summary>
        /// <param name="platform"><see cref="PushNotificationPlatform"/></param>
        /// <param name="notification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref="NotificationJsonBase"/></returns>
        public static NotificationJsonBase Extract(PushNotificationPlatform platform, SwabbrNotification notification)
        {
            if (notification == null) { throw new ArgumentNullException(nameof(notification)); }

            // TODO Why do we need this?
            //var notificationPayload = ExtractPayload(notification);

            return platform switch
            {
                PushNotificationPlatform.APNS => FcmJsonExtractor.Extract(notification),
                PushNotificationPlatform.FCM => ApnsJsonExtractor.Extract(notification),
                _ => throw new InvalidOperationException(nameof(platform)),
            };
        }

        // TODO Why do we need this?
        ///// <summary>
        ///// Extracts a <see cref="NotificationPayloadJsonWrapper"/> from a given
        ///// <paramref name="notification"/>.
        ///// </summary>
        ///// <param name="notification"><see cref="SwabbrNotification"/></param>
        ///// <returns><see cref="NotificationPayloadJsonWrapper"/></returns>
        //private NotificationPayloadJsonWrapper ExtractPayload(SwabbrNotification notification)
        //{
        //    if (notification == null) { throw new ArgumentNullException(nameof(notification)); }

        //    return new NotificationPayloadJsonWrapper
        //    {
        //        Body = notification.Body,
        //        CreatedAt = notification.CreatedAt,
        //        DataType = "TODO FIX MyDataType",
        //        DataTypeVersion = "TODO FIX MyDataTypeVersion",
        //        NotificationAction = NotificationActionMapper.Map(notification.NotificationAction),
        //        Title = notification.Title,
        //        Pars = notification.Pars // TODO Maybe implement separate JSON objects for this (this actually exposes the core)
        //    };
        //}

    }

}
