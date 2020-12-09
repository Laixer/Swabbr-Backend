﻿using Swabbr.Core.DataAnnotations;
using Swabbr.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog.
    /// </summary>
    public class VlogDto
    {
        /// <summary>
        ///     Unique vlog identifier.
        /// </summary>
        [Required]
        [NonEmptyGuid]
        public Guid Id { get; set; }

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
        ///     The length of this vlog in seconds.
        /// </summary>
        public uint? LengthInSeconds { get; set; }

        /// <summary>
        ///     Indicates the state of this vlog.
        /// </summary>
        public VlogStatus VlogStatus { get; set; }

        /// <summary>
        ///     Vlog video download uri.
        /// </summary>
        public Uri VideoUri { get; set; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }
    }
}