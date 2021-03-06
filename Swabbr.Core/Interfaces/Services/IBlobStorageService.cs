﻿using Swabbr.Core.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Store a file contents in a data store.
    /// </summary>
    public interface IBlobStorageService : ITestableService
    {
        /// <summary>
        ///     Check if a file exist in storage.
        /// </summary>
        /// <param name="containerName">Storage container.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>True if file exist, false otherwise.</returns>
        Task<bool> FileExistsAsync(string containerName, string fileName);

        /// <summary>
        ///     Retrieve file access link as uri.
        /// </summary>
        /// <param name="containerName">Storage container.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="timeSpanValid">How long the link is valid.</param>
        /// <returns>The generated link.</returns>
        Task<Uri> GetAccessLinkAsync(string containerName, string fileName, TimeSpan timeSpanValid);

        /// <summary>
        ///     Generate a signed uri to upload a file.
        /// </summary>
        /// <param name="containerName">Storage container.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="timeSpanValid">How long the link is valid.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>The generated link.</returns>
        Task<Uri> GenerateUploadLinkAsync(string containerName, string fileName, TimeSpan timeSpanValid, string contentType);

        /// <summary>
        ///     Store the file in the data store.
        /// </summary>
        /// <param name="containerName">Storage container.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="stream">Content stream.</param>
        Task StoreFileAsync(string containerName, string fileName, Stream stream);

        // FUTURE: Refactor
        /// <summary>
        ///     Stores a file in a Digital Ocean Space.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="stream">See <see cref="Stream"/>.</param>
        /// <param name="storageObject">Storage object settings.</param>
        /// <returns>See <see cref="ValueTask"/>.</returns>
        Task StoreFileAsync(string containerName, string fileName, string contentType, Stream stream, StorageObject storageObject = null);
    }
}
