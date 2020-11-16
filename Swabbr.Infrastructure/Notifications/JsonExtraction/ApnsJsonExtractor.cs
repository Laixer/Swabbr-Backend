using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Utility;
using System;

namespace Swabbr.Core.Notifications.JsonExtraction
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
        public static NotificationJsonBase Extract(SwabbrNotification swabbrNotification)
        {
            if (swabbrNotification == null)
            {
                throw new ArgumentNullException(nameof(swabbrNotification));
            }
            swabbrNotification.ThrowIfInvalid();

            return new ApnsContentWrapper
            {
                Aps = new ApnsContentAps
                {
                    Alert = new ApnsContentAlert
                    {
                        // TODO Implement
                    }
                }
            };

            throw new NotImplementedException();
        }
    }
}
