using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Base class for a request that is sent to a user.
    /// </summary>
    public abstract class Request : EntityBase<Guid>
    {

        /// <summary>
        /// The id of the <see cref="SwabbrUser"/> that should request the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The moment this request was created.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// The last moment the state was updated.
        /// </summary>
        public DateTimeOffset StateUpdateDate { get; set; }

        /// <summary>
        /// Indicates the state of this request.
        /// </summary>
        public RequestState RequestState { get; set; }

    }

}
