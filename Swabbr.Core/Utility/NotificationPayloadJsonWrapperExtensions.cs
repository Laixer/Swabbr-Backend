using Laixer.Utility.Extensions;
using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Utility
{
    public static class NotificationPayloadJsonWrapperExtensions
    {

        /// <summary>
        /// Throws if the given <paramref name="wrapper"/> is invalid.
        /// </summary>
        /// <param name="wrapper"></param>
        public static void ThrowIfInvalid(this NotificationPayloadJsonWrapper wrapper)
        {
            if (wrapper == null) { throw new ArgumentNullException(nameof(wrapper)); }
            wrapper.Body.ThrowIfNullOrEmpty();
            if (wrapper.CreatedAt == null) { throw new ArgumentNullException("No create time"); }
            wrapper.DataType.ThrowIfNullOrEmpty();
            wrapper.DataTypeVersion.ThrowIfNullOrEmpty();
            wrapper.NotificationAction.ThrowIfNullOrEmpty();
            wrapper.Protocol.ThrowIfNullOrEmpty();
            wrapper.ProtocolVersion.ThrowIfNullOrEmpty();
            wrapper.Title.ThrowIfNullOrEmpty();
        }

    }
}
