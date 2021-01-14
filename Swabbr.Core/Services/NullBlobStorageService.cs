using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Blob storage service which does nothing.
    /// </summary>
    public class NullBlobStorageService : IBlobStorageService
    {
        /// <summary>
        ///     This always returns true.
        /// </summary>
        public Task<bool> FileExistsAsync(string containerName, string fileName) => Task.FromResult(true);

        /// <summary>
        ///     Returns a <c>null</c> object.
        /// </summary>
        public Task<Uri> GetAccessLinkAsync(string containerName, string fileName, TimeSpan timeSpanValid) => null;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task StoreFileAsync(string containerName, string fileName, Stream stream) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task StoreFileAsync(string containerName, string fileName, string contentType, Stream stream, StorageObject storageObject = null) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task TestServiceAsync() => Task.CompletedTask;
    }
}
