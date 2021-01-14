using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Notifications.JsonWrappers;
using System;

namespace Swabbr.Infrastructure.Notifications.JsonExtraction
{
    /// <summary>
    ///     Contains functionality for creating properly formatted JSON objects to 
    ///     be sent to our Azure Notification Hub for APNS.
    /// </summary>
    internal static class ApnsJsonExtractor
    {
        /// <summary>
        ///     Formats a notifcation for Apple Push Notification Platform
        ///     usage through our Azure Notification Hub.
        /// </summary>
        /// <param name="swabbrNotification">The notification.</param>
        /// <returns>Formatted notification.</returns>
        public static NotificationWrapperJsonBase Extract(SwabbrNotification swabbrNotification)
        {
            if (swabbrNotification == null)
            {
                throw new ArgumentNullException(nameof(swabbrNotification));
            }

            return new ApnsContentWrapper
            {
                Aps = new ApnsContentAps
                {
                    Alert = new ApnsContentAlert
                    {
                        // FUTURE Implement and test
                    }
                }
            };

            throw new NotImplementedException();
        }
    }
}
