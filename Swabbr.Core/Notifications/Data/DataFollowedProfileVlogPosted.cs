﻿using System;

namespace Swabbr.Core.Notifications.Data
{
    /// <summary>
    ///     Data wrapper for notifying about a posted vlog.
    /// </summary>
    public sealed class DataFollowedProfileVlogPosted : NotificationData
    {
        /// <summary>
        /// Internal vlog id.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Internal user id of the person that owns the vlog.
        /// </summary>
        public Guid VlogOwnerUserId { get; set; }
    }
}
