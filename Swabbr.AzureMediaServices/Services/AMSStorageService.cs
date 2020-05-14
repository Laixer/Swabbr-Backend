using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Handles assets and cleanups for Azure Media Services.
    /// TODO Move AMS stuff to <see cref="Interfaces.Clients.IAMSClient"/>.
    /// </summary>
    public sealed class AMSStorageService : IStorageService
    {

        private readonly AMSConfiguration config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSStorageService(IOptions<AMSConfiguration> options)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value; 
        }

        /// <summary>
        /// Cleans up the remains of a reaction transcoding process in Azure
        /// Media Services. 
        /// </summary>
        /// <remarks>
        /// - Removes the input asset
        /// - Removes the job
        /// 
        /// TODO Maybe check if a job is actually finished before doing this?
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupReactionStorageOnSuccessAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);

            // Delete input asset
            // TODO Delete or not? Depends on our codec choice
            await amsClient.Assets.DeleteAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reactionId)).ConfigureAwait(false);

            // Delete job
            await amsClient.Jobs.DeleteAsync(config.ResourceGroup, config.AccountName, AMSNameConstants.ReactionTransformName,
                AMSNameGenerator.ReactionJobName(reactionId)).ConfigureAwait(false);
        }

        /// <summary>
        /// Cleans up the remains of a reaction transcoding process in Azure
        /// Media Services after we failed.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupReactionStorageOnFailureAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);

            // Delete assets

            // We don't want to delete the input asset (keep the data)
            //await amsClient.Assets.DeleteAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reactionId)).ConfigureAwait(false);
            await amsClient.Assets.DeleteAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionOutputAssetName(reactionId)).ConfigureAwait(false);

            // Delete job
            await amsClient.Jobs.DeleteAsync(config.ResourceGroup, config.AccountName, AMSNameConstants.ReactionTransformName,
                AMSNameGenerator.ReactionJobName(reactionId)).ConfigureAwait(false);
        }

        /// <summary>
        /// Cleans up the storage for a <see cref="Core.Entities.Vlog"/>.
        /// </summary>
        /// <remarks>
        /// This does not delete the <see cref="Core.Entities.Vlog"/>.
        /// At the moment this does nothing, because we want to keep our data.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task CleanupVlogStorageOnDeleteAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            return Task.CompletedTask;
            //var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            //var vlogAssetName = AMSNameGenerator.VlogLiveOutputAssetName(vlogId);
            //await amsClient.Assets.DeleteAsync(config.ResourceGroup, config.AccountName, vlogAssetName).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new SAS for a reaction thumbnail.
        /// </summary>
        /// <remarks>
        /// TODO Implement specific
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        public Task<Uri> GetDownloadAccessUriForReactionThumbnailAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetDownloadAccessUriForReactionContainerAsync(reactionId);
        }

        /// <summary>
        /// Creates a new SAS for a reaction video.
        /// </summary>
        /// <remarks>
        /// TODO Implement specific
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        public Task<Uri> GetDownloadAccessUriForReactionVideoAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetDownloadAccessUriForReactionContainerAsync(reactionId);
        }

        /// <summary>
        /// Creates a new SAS for a reaction container.
        /// </summary>
        /// <remarks>
        /// The created SAS is valid for 15 minutes.
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        public async Task<Uri> GetDownloadAccessUriForReactionContainerAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var outputAssetName = AMSNameGenerator.ReactionOutputAssetName(reactionId);
            var assetContainerSas = await amsClient.Assets.ListContainerSasAsync(
                    config.ResourceGroup,
                    config.AccountName,
                    outputAssetName,
                    permissions: AssetContainerPermission.Read,
                    expiryTime: DateTime.UtcNow.AddMinutes(30).ToUniversalTime()) // TODO Make configable
                .ConfigureAwait(false);

            if (!assetContainerSas.AssetContainerSasUrls.Any()) { throw new ExternalErrorException($"Could not find output asset with name {outputAssetName} in Azure Media Services"); }

            return new Uri(assetContainerSas.AssetContainerSasUrls.FirstOrDefault()); // First access key
        }

    }

}
