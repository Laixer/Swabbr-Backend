﻿using System;

namespace Swabbr.Core.Storage
{
    // FUTURE This might be too specific for S3.
    /// <summary>
    ///     Options for the blob storage service.
    /// </summary>
    public sealed class BlobStorageOptions
    {
        /// <summary>
        ///     Base service uri for blob storage service.
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        ///     Name of the blob storage.
        /// </summary>
        public string BlobStorageName { get; set; }

        /// <summary>
        ///     Public access key.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        ///     Private secret key.
        /// </summary>
        public string SecretKey { get; set; }
    }
}
