using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Functions as the primary key for a follow request.
    /// </summary>
    public sealed class FollowRequestId
    {

        /// <summary>
        /// Id of the requesting user.
        /// </summary>
        public Guid RequesterId { get; set; }

        /// <summary>
        /// Id of the receiving user.
        /// </summary>
        public Guid ReceiverId { get; set; }

    }

}
