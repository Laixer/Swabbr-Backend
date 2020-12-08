using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Contains notification details for a user.
    /// </summary>
    public sealed class UserPushNotificationDetails
    {
        /// <summary>
        ///     The user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The notification platform of the user.
        /// </summary>
        public PushNotificationPlatform PushNotificationPlatform { get; set; }
    }
}
