﻿using Swabbr.Core.Notifications;
using Swabbr.Core.Helpers;
using Swabbr.Infrastructure.Notifications.JsonWrappers;
using System;

namespace Swabbr.Infrastructure.Notifications.JsonExtraction
{
    /// <summary>
    ///     Contains functionality for creating properly formatted JSON objects to 
    ///     be sent to our Azure Notification Hub for Firebase.
    /// </summary>
    internal static class FcmJsonExtractor
    {
        /// <summary>
        ///     Formats a <see cref="FcmContentWrapper"/> for Google Firebase based
        ///     on a given <paramref name="swabbrNotification"/>.
        /// </summary>
        /// <param name="swabbrNotification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref="FcmContentWrapper"/></returns>
        public static NotificationWrapperJsonBase Extract(SwabbrNotification swabbrNotification)
        {
            if (swabbrNotification == null)
            {
                throw new ArgumentNullException(nameof(swabbrNotification));
            }

            // TODO Validate notification?

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
