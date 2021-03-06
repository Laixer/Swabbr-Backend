﻿using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#pragma warning disable CA1812 // Internal class is never instantiated
namespace Swabbr.Infrastructure.Storage
{
    /// <summary>
    ///     Amazon S3 implementation of <see cref="IBlobStorageService"/> which
    ///     is also compatible with Digital Ocean Spaces.
    /// </summary>
    /// <remarks>
    ///     This creates an <see cref="IAmazonS3"/> client once in its constructor.
    ///     Register this service as a singleton if dependency injection is used.
    /// </remarks>
    internal class SpacesBlobStorageService : IBlobStorageService, IDisposable
    {
        private readonly BlobStorageOptions _options;
        private readonly IAmazonS3 client;
        private readonly ILogger<SpacesBlobStorageService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public SpacesBlobStorageService(IOptions<BlobStorageOptions> options,
            ILogger<SpacesBlobStorageService> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create client once.
            client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.SecretKey),
                new AmazonS3Config
                {
                    ServiceURL = _options.ServiceUri.AbsoluteUri
                });
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose() => client.Dispose();

        // FUTURE: Look into different approaches for this.
        /// <summary>
        ///     Checks if a file exists or not.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>Boolean result.</returns>
        public async Task<bool> FileExistsAsync(string containerName, string fileName)
        {
            try
            {
                var result = await client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _options.BlobStorageName,
                    Key = string.IsNullOrEmpty(containerName) ? fileName : $"{containerName}/{fileName}"
                });

                return true;
            }
            catch (Exception e)
            {
                // This type of exception indicates that the file does not exist.
                if (e is AmazonS3Exception exception && exception.ErrorCode == "NoSuchKey")
                {
                    return false;
                }

                _logger.LogError(e, "Could not check file existence in Spaces using S3");

                throw new StorageException("Could not check file existence", e);
            }
        }

        // TODO Look at duplicate code
        /// <summary>
        ///     Generates a signed upload uri.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="timeSpanValid">How long the link is valid.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>Signed upload uri.</returns>
        public Task<Uri> GenerateUploadLinkAsync(string containerName, string fileName, TimeSpan timeSpanValid, string contentType)
        {
            try
            {
                var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = _options.BlobStorageName,
                    Key = string.IsNullOrEmpty(containerName) ? fileName : $"{containerName}/{fileName}",
                    Expires = DateTime.UtcNow.Add(timeSpanValid),
                    Verb = HttpVerb.PUT,
                    ContentType = contentType
                });

                return Task.FromResult(new Uri(url));
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Could not generate upload uri for Spaces using S3");

                throw new StorageException("Could not generate upload uri", e);
            }
        }

        /// <summary>
        ///     Gets an access uri for a given file.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="timeSpanValid">How long the link is valid.</param>
        /// <returns>Access <see cref="Uri"/>.</returns>
        public Task<Uri> GetAccessLinkAsync(string containerName, string fileName, TimeSpan timeSpanValid)
        {
            try
            {
                var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = _options.BlobStorageName,
                    Key = string.IsNullOrEmpty(containerName) ? fileName : $"{containerName}/{fileName}",
                    Expires = DateTime.UtcNow.Add(timeSpanValid),
                    Verb = HttpVerb.GET
                });

                return Task.FromResult(new Uri(url));
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Could not get access link from Spaces using S3");

                throw new StorageException("Could not get access link", e);
            }
        }

        /// <summary>
        ///     Stores a file in a Digital Ocean Space.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="stream">See <see cref="Stream"/>.</param>
        /// <returns>See <see cref="ValueTask"/>.</returns>
        public async Task StoreFileAsync(string containerName, string fileName, Stream stream)
        {
            try
            {
                var key = string.IsNullOrEmpty(containerName) ? fileName : $"{containerName}/{fileName}";
                using var transferUtility = new TransferUtility(client);

                await transferUtility.UploadAsync(stream, _options.BlobStorageName, key);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Could not store file to Spaces using S3");

                throw new StorageException("Could not store file", e);
            }
        }

        /// <summary>
        ///     Stores a file in a Digital Ocean Space.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="stream">See <see cref="Stream"/>.</param>
        /// <param name="storageObject">Storage object settings.</param>
        /// <returns>See <see cref="ValueTask"/>.</returns>
        public async Task StoreFileAsync(string containerName, string fileName, string contentType, Stream stream, StorageObject storageObject)
        {
            try
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _options.BlobStorageName,
                    ContentType = contentType,
                    Key = string.IsNullOrEmpty(containerName) ? fileName : $"{containerName}/{fileName}",
                    InputStream = stream,
                };

                if (storageObject != null)
                {
                    request.CannedACL = storageObject.IsPublic ? S3CannedACL.PublicRead : S3CannedACL.Private;
                    request.Headers.ContentType = storageObject.ContentType ?? request.Headers.ContentType;
                    request.Headers.CacheControl = storageObject.CacheControl ?? request.Headers.CacheControl;
                    request.Headers.ContentDisposition = storageObject.ContentDisposition ?? request.Headers.ContentDisposition;
                    request.Headers.ContentEncoding = storageObject.ContentEncoding ?? request.Headers.ContentEncoding;
                }

                using var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, $"Could not store file with content type {contentType} to Spaces using S3");

                throw new StorageException($"Could not upload file with content type {contentType}", e);
            }
        }

        /// <summary>
        ///     Test the Amazon S3 service backend.
        /// </summary>
        public async Task TestServiceAsync()
            => await client.ListBucketsAsync();
    }
}
#pragma warning restore CA1812 // Internal class is never instantiated
