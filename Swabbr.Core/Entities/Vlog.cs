using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     A vlog created by a user.
    /// </summary>
    public class Vlog : VideoBase
    {
        /// <summary>
        ///     Id of the user who created the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     The date at which the recording of the vlog started.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Total amount of views for this vlog.
        /// </summary>
        public uint Views { get; set; }

        /// <summary>
        ///     Indicates the state of this vlog.
        /// </summary>
        public VlogStatus VlogStatus { get; set; }
    }
}
