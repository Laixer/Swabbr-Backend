using Laixer.Utility.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{
    /// <summary>
    /// Handles assets and cleanups for Azure Media Services.
    /// </summary>
    public sealed class AMSStorageService : IStorageService
    {
        private readonly IAMSClient _amsClient;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSStorageService(IAMSClient amsClient) => _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));

        /// <summary>
        /// Cleans up the remains of a reaction transcoding process in Azure
        /// Media Services. 
        /// </summary>
        /// <remarks>
        /// - Removes the input asset
        /// - Removes the job
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupReactionStorageOnSuccessAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            // TODO Check if job is finished before doing this

            // Delete input asset
            await _amsClient.DeleteAssetAsync(AMSNameGenerator.ReactionInputAssetName(reactionId)).ConfigureAwait(false);

            // Delete job
            await _amsClient.DeleteReactionJobAsync(reactionId).ConfigureAwait(false);
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

            // Delete assets
            await _amsClient.DeleteAssetAsync(AMSNameGenerator.ReactionOutputAssetName(reactionId)).ConfigureAwait(false);

            // Delete job
            await _amsClient.DeleteReactionJobAsync(reactionId).ConfigureAwait(false);
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
        /// Creates a new SAS for a reaction output container.
        /// </summary>
        /// <remarks>
        /// The created SAS is valid for <see cref="AMSConstants.ReactionSasOutputExpireTimeMinutes"/> minutes.
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        public Task<Uri> GetDownloadAccessUriForReactionContainerAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return _amsClient.GetReactionOutputAssetSasAsync(reactionId);
        }

        public Task<Uri> GetDownloadAccessUriForVlogThumbnailAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            return _amsClient.GetVlogOutputAssetSasAysync(vlogId);
        }
    }
}
