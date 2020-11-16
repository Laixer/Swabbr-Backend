using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLike : EntityBase<VlogLikeId>
    {
        /// <summary>
        ///     The time at which the user liked the vlog.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }
    }
}
