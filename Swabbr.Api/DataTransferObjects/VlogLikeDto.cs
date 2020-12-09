using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog like.
    /// </summary>
    public class VlogLikeDto
    {
        /// <summary>
        ///     The internal vlog id.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     The internal user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The time at which the user liked the vlog.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }
    }
}
