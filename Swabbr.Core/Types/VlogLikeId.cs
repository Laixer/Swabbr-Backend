using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains the primary key information for a <see cref="Entities.VlogLike"/>.
    /// </summary>
    public sealed class VlogLikeId
    {

        public Guid VlogId { get; set; }

        public Guid UserId { get; set; }

    }

}
