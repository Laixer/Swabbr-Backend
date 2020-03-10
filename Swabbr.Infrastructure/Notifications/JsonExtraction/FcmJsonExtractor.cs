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
        /// Formats a <see cref="NotificationJsonBase"/> for Google Firebase based
        /// on a given <paramref name="payloadJsonWrapper"/>.
        /// </summary>
        /// <param name="payloadJsonWrapper"><see cref="NotificationPayloadJsonWrapper"/></param>
        /// <returns><see cref="FcmContentWrapper"/></returns>
        public NotificationJsonBase Extract(NotificationPayloadJsonWrapper payloadJsonWrapper)
        {
            if (payloadJsonWrapper == null) { throw new ArgumentNullException(nameof(payloadJsonWrapper)); }
            payloadJsonWrapper.ThrowIfInvalid();

            return new FcmContentWrapper
            {
                Data = new SubData
                {
                    Payload = payloadJsonWrapper
                }
            };
        }

    }

}
