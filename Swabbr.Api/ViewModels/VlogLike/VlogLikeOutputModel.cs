﻿using System;

namespace Swabbr.Api.ViewModels.VlogLike
{

    /// <summary>
    /// Represents a vlog like.
    /// </summary>
    public sealed class VlogLikeOutputModel
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