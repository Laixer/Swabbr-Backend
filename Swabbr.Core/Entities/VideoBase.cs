using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Base class for video on demand.
    /// </summary>
    public abstract class VideoBase : EntityBase<Guid>
    {
        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }

        /// <summary>
        ///     Video content download uri.
        /// </summary>
        public Uri VideoUri { get; set; }

        /// <summary>
        ///     Video length in seconds.
        /// </summary>
        public uint? Length { get; set; }
    }
}
