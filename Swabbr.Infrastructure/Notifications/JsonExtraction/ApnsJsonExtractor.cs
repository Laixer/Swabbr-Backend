using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using System;

namespace Swabbr.Core.Notifications.JsonExtraction
{

    /// <summary>
    /// Contains functionality for creating properly formatted JSON objects to 
    /// be sent to our Azure Notification Hub for Firebase.
    /// </summary>
    internal sealed class ApnsJsonExtractor : IPlatformSpecificJsonExtractor
    {

        public NotificationJsonBase Extract(SwabbrNotification swabbrNotification)
        {
            if (swabbrNotification == null) { throw new ArgumentNullException(nameof(swabbrNotification)); }
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
