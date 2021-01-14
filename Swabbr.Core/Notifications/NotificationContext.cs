using System;

namespace Swabbr.Core.Notifications
{
    /// <summary>
    ///     Context for sending a notification to one
    ///     or multiple users.
    /// </summary>
    public class NotificationContext
    {
        /// <summary>
        ///     The user that will be notified.
        /// </summary>
        /// <remarks>
        ///     This can be ignored if we plan on 
        ///     notifying multiple users, leave it
        ///     as <see cref="Guid.Empty"/>.
        /// </remarks>
        public Guid NotifiedUserId { get; set; }

        /// <summary>
        ///     The notification object itself.
        /// </summary>
        public SwabbrNotification Notification { get; set; }

        /// <summary>
        ///     Date when the notification was sent.
        /// </summary>
        public DateTimeOffset DateSent { get; set; }

        /// <summary>
        ///     True if a user has been assigned.
        /// </summary>
        public bool HasUser => NotifiedUserId != Guid.Empty;
    }
}
