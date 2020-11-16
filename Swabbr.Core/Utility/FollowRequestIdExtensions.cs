using Swabbr.Core.Extensions;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extensions for the <see cref="FollowRequestId"/> class.
    /// </summary>
    public static class FollowRequestIdExtensions
    {

        /// <summary>
        /// Checks if a <paramref name="id"/> object is null or contains any
        /// null or empty <see cref="Guid"/>s. This will throw if any of these 
        /// items is null or empty.
        /// </summary>
        /// <param name="id"><see cref="FollowRequestId"/></param>
        public static void ThrowIfNullOrEmpty(this FollowRequestId id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            id.RequesterId.ThrowIfNullOrEmpty();
            id.ReceiverId.ThrowIfNullOrEmpty();
        }

    }
}
