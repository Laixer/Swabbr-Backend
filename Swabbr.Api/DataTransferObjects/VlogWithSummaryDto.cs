using System.Collections.Generic;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog with a vlog like summary.
    /// </summary>
    public class VlogWithSummaryDto : VlogDto
    {
        /// <summary>
        ///     The total amount of likes for this vlog.
        /// </summary>
        public uint TotalLikes { get; set; }

        /// <summary>
        ///     A few users that liked this vlog.
        /// </summary>
        /// <remarks>
        ///     This does not need to contain all the users.
        /// </remarks>
        public IEnumerable<UserDto> Users { get; set; }
    }
}
