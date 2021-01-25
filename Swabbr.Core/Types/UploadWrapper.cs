using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Wrapper around upload details for a video.
    /// </summary>
    public record UploadWrapper
    {
        /// <summary>
        ///     Not-yet existing video id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Pre-signed video upload uri.
        /// </summary>
        public Uri VideoUploadUri { get; set; }

        /// <summary>
        ///     Pre-signed thumbnail upload uri.
        /// </summary>
        public Uri ThumbnailUploadUri { get; set; }
    }
}
