using Swabbr.Core.Extensions;
using Swabbr.Core.Notifications;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extension functionality for <see cref="SwabbrNotification"/>.
    /// </summary>
    public static class SwabbrNotificationExtensions
    {

        /// <summary>
        /// Throws if the given <paramref name="wrapper"/> is invalid.
        /// </summary>
        /// <param name="wrapper"></param>
        public static void ThrowIfInvalid(this SwabbrNotification wrapper)
        {
            if (wrapper == null) { throw new ArgumentNullException(nameof(wrapper)); }
            if (wrapper.Timestamp.IsNullOrEmpty()) { throw new ArgumentNullException("No create time"); }
            wrapper.DataType.ThrowIfNullOrEmpty();
            wrapper.DataTypeVersion.ThrowIfNullOrEmpty();
            wrapper.ClickAction.ThrowIfNullOrEmpty();
            wrapper.Protocol.ThrowIfNullOrEmpty();
            wrapper.ProtocolVersion.ThrowIfNullOrEmpty();
            wrapper.ContentType.ThrowIfNullOrEmpty();
            if (wrapper.Data == null) { throw new ArgumentNullException(nameof(wrapper.Data)); }
        }

    }
}
