using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents a single registration event for a device to receive notifications.
    /// </summary>
    public class NotificationRegistration : EntityBase<Guid>
    {

        /// <summary>
        /// External registration id.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Id of the user this registration is bound to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// PNS handle of the device
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Indicates which platform is being used for sending push notifications.
        /// </summary>
        public PushNotificationPlatform PushNotificationPlatform { get; set; }

    }

}
