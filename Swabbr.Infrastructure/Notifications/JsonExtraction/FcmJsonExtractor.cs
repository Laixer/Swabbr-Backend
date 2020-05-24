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
    internal sealed class FcmJsonExtractor : IPlatformSpecificJsonExtractor
    {

        /// <summary>
        /// Formats a <see cref="FcmContentWrapper"/> for Google Firebase based
        /// on a given <paramref name="swabbrNotification"/>.
        /// </summary>
        /// <param name="swabbrNotification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref="FcmContentWrapper"/></returns>
        public NotificationJsonBase Extract(SwabbrNotification swabbrNotification)
        {
            if (swabbrNotification == null) { throw new ArgumentNullException(nameof(swabbrNotification)); }
            swabbrNotification.ThrowIfInvalid();

            return new FcmContentWrapper
            {
                Data = new SubData
                {
                    Payload = swabbrNotification
                }
            };
        }

    }

}
