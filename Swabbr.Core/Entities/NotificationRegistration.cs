using Swabbr.Core.Notifications;
using System;

namespace Swabbr.Core.Entities
{
    public class NotificationRegistration : EntityBase
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
        /// Indicates which platform is being used for sending push notifications.
        /// </summary>
        public PushNotificationPlatform Platform { get; set; }
    }
}
