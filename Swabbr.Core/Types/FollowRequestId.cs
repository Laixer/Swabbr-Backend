using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Id object containing a requesting
    ///     user id and a receiving user id.
    /// </summary>
    public sealed class FollowRequestId
    {
        /// <summary>
        ///     Id of the requesting user.
        /// </summary>
        public Guid RequesterId { get; set; }

        /// <summary>
        ///     Id of the receiving user.
        /// </summary>
        public Guid ReceiverId { get; set; }
    }
}
