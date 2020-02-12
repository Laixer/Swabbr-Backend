﻿using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLike : EntityBase
    {

        /// <summary>
        /// Id of the vlog that was given a like.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user that created the like.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The time at which the user liked the vlog.
        /// </summary>
        public DateTimeOffset TimeCreated { get; set; }

    }

}
