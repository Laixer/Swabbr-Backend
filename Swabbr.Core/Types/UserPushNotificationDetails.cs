using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Composite class that contains a <see cref="Entities.SwabbrUser"/> id 
    /// along with the <see cref="Entities.NotificationRegistration"/> platform.
    /// </summary>
    public sealed class UserPushNotificationDetails
    {

        public Guid UserId { get; set; }

        public PushNotificationPlatform PushNotificationPlatform { get; set; }

    }
}
