using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLike : EntityBase<VlogLikeId>
    {
        ///// <summary>
        ///// TODO This seems like a beunfix.
        ///// When we parse this object into it's Id object through UserId and VlogId,
        ///// the Id object is still null.
        ///// </summary>
        //public VlogLike() => Id = new VlogLikeId();

        /// <summary>
        ///     The time at which the user liked the vlog.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }
    }
}
