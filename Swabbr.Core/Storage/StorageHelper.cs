using System;

namespace Swabbr.Core.Storage
{
    /// <summary>
    ///     Contains generalized utility functionality
    ///     for entity file names.
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        ///     Generates the filename for a video based
        ///     on a vlog or reaction id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The generated video file name.</returns>
        public static string GetVideoFileName(Guid id)
            => $"{id}";

        /// <summary>
        ///     Generates the filename for a thumbnail
        ///     based on a vlog or reaction id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The generated thumbnail file name.</returns>
        public static string GetThumbnailFileName(Guid id)
            => $"{id}-thumbnail";
    }
}
