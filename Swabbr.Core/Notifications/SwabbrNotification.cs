using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Represents a single notification to be sent to a user.
    /// </summary>
    public sealed class SwabbrNotification
    {

        /// <summary>
        /// Constructor to force us to always initialize <see cref="NotificationAction"/>.
        /// </summary>
        /// <param name="notificationAction"><see cref="NotificationAction"/></param>
        public SwabbrNotification(NotificationAction notificationAction)
        {
            NotificationAction = notificationAction;
            CreatedAt = DateTimeOffset.Now;
        }

        public string Title { get; set; }

        public string Body { get; set; }

        public NotificationAction NotificationAction { get; } // TODO Is this allowed? Is this correct?

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now; // TODO Is this correct?

        /// <summary>
        /// Contains action specific values.
        /// </summary>
        public ParametersJsonBase Pars { get; set; }

    }

}
