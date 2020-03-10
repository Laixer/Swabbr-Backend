using Laixer.Utility.Extensions;
using Swabbr.Core.Notifications.JsonWrappers;
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

        /// <summary>
        /// Formats a <see cref="NotificationJsonBase"/> for Apple Push Notification 
        /// Services based on a given <paramref name="notification"/>.
        /// </summary>
        /// <param name="notification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref="ApnsContentWrapper"/></returns>
        public NotificationJsonBase Extract(NotificationPayloadJsonWrapper payloadJsonWrapper)
        {
            throw new NotImplementedException();
        }

    }

}
