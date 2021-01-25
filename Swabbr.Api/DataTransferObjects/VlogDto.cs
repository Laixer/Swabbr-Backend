using Swabbr.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog.
    /// </summary>
    public record VlogDto
    {
        /// <summary>
        ///     Unique vlog identifier.
        /// </summary>
        [Required]
        public Guid Id { get; init; }

        /// <summary>
        ///     Id of the user who created the vlog.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        ///     Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; init; }

        /// <summary>
        ///     The date at which the recording of the vlog started.
        /// </summary>
        public DateTimeOffset DateCreated { get; init; }

        /// <summary>
        ///     Total amount of views for this vlog.
        /// </summary>
        public uint Views { get; init; }

        /// <summary>
        ///     The length of this vlog in seconds.
        /// </summary>
        public uint? Length { get; init; }

        /// <summary>
        ///     Indicates the state of this vlog.
        /// </summary>
        public VlogStatus VlogStatus { get; init; }

        /// <summary>
        ///     Vlog video download uri.
        /// </summary>
        public Uri VideoUri { get; init; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; init; }
    }
}
