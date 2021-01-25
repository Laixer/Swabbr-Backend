using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for uploading a new vlog or reaction.
    /// </summary>
    public class UploadWrapperDto
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
