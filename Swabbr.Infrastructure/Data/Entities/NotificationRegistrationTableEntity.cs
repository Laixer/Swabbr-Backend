using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Notifications;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the storage data of a notification registration
    /// </summary>
    public class NotificationRegistrationTableEntity : TableEntity
    {
        /// <summary>
        /// Unique registration identifier
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Id of the user this registration is bound to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// PNS handle of the device
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The integer representation of the <see cref="PushNotificationPlatform"/> value for this registration.
        /// Indicates which platform is being used for sending push notifications.
        /// </summary>
        public int Platform { get; set; }
    }
}
