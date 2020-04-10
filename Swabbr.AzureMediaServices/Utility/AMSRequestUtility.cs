using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Swabbr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Contains functionality to process AMS segmented requests in a clean way.
    /// </summary>
    internal static class AMSRequestUtility
    {

        /// <summary>
        /// Lists all blobs in a container.
        /// </summary>
        /// <param name="container"><see cref="CloudBlobContainer"/></param>
        /// <returns><see cref="CloudBlob"/> collection</returns>
        internal static async Task<IEnumerable<CloudBlockBlob>> ListBlobsAsync(CloudBlobContainer container)
        {
            if (container == null) { throw new ArgumentNullException(nameof(container)); }

            try
            {
                var continuationToken = null as BlobContinuationToken;
                var result = new Collection<CloudBlockBlob>();

                do
                {
                    var resultSegment = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, 1000, continuationToken, null, null);

                    foreach (var blobItem in resultSegment.Results)
                    {
                        result.Add((CloudBlockBlob)blobItem);
                    }

                    // Get the continuation token and loop until it is null.
                    continuationToken = resultSegment.ContinuationToken;
                }
                while (continuationToken != null);

                return result;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw new ExternalErrorException($"Error while listing all blobs for container");
            }
        }

    }

}
