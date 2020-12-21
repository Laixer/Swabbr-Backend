using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    // TODO DateCreated
    /// <summary>
    ///     Represents a single registration event for a 
    ///     device to receive notifications.
    /// </summary>
    /// <remarks>
    ///     The <see cref="EntityBase{Guid}.Id"/> property 
    ///     represents the user id.
    /// </remarks>
    public class NotificationRegistration : EntityBase<Guid>
    {
        /// <summary>
        ///     External registration id.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        ///     PNS handle of the device
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        ///     Indicates which platform is being used for sending push notifications.
        /// </summary>
        public PushNotificationPlatform PushNotificationPlatform { get; set; }
    }
}
