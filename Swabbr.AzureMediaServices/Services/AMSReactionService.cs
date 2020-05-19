using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Transcoding wrapper based on Azure Media Services.
    /// This has no knowlede of our data store.
    /// </summary>
    public sealed class AMSReactionService : IReactionService
    {

        private readonly IAMSClient _amsClient;
        private readonly IReactionRepository _reactionRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IStorageService _storageService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly AMSConfiguration config;
        private readonly SwabbrConfiguration swabbrConfiguration;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSReactionService(IAMSClient amsClient,
            IReactionRepository reactionRepository,
            IVlogRepository vlogRepository,
            IUserRepository userRepository,
            IStorageService storageService,
            INotificationService notificationService,
            IOptions<AMSConfiguration> optionsAms,
            IOptions<SwabbrConfiguration> optionsSwabbr)
        {
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            if (optionsAms == null) { throw new ArgumentNullException(nameof(optionsAms)); }
            optionsAms.Value.ThrowIfInvalid();
            config = optionsAms.Value;

            if (optionsSwabbr == null) { throw new ArgumentNullException(nameof(optionsSwabbr)); }
            optionsSwabbr.Value.ThrowIfInvalid();
            swabbrConfiguration = optionsSwabbr.Value;
        }

        /// <summary>
        /// Soft deletes a <see cref="Reaction"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteReactionAsync(Guid userId, Guid reactionId)
        {
            userId.ThrowIfNullOrEmpty();
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var reaction = await GetReactionAsync(reactionId).ConfigureAwait(false);
                if (reaction.UserId != userId) { throw new UserNotOwnerException(nameof(reaction)); }
                if (reaction.ReactionState != ReactionState.Finished) { throw new ReactionStateException(ReactionState.Finished); }

                await _reactionRepository.SoftDeleteAsync(reactionId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Gets a new SAS Uri for a <see cref="Reaction"/> upload.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        public async Task<Uri> GetNewUploadUriAsync(Guid userId, Guid reactionId)
        {
            userId.ThrowIfNullOrEmpty();
            reactionId.ThrowIfNullOrEmpty();

            var reaction = await GetReactionAsync(reactionId).ConfigureAwait(false);
            if (reaction.ReactionState != ReactionState.Created) { throw new ReactionStateException($"Reaction not in {ReactionState.Created.GetEnumMemberAttribute()} state"); }
            if (reaction.UserId != userId) { throw new UserNotOwnerException(nameof(reaction)); }

            // TODO DRY
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            await amsClient.Assets.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reaction.Id), new Asset()).ConfigureAwait(false);
            var sas = await amsClient.Assets.ListContainerSasAsync(config.ResourceGroup, config.AccountName,
                AMSNameGenerator.ReactionInputAssetName(reaction.Id),
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddMinutes(15).ToUniversalTime()).ConfigureAwait(false);
            return new Uri(sas.AssetContainerSasUrls.First());
        }

        /// <summary>
        /// Gets the <see cref="SwabbrUser"/> owner of a <see cref="Vlog"/> to
        /// which a given <see cref="Reaction"/> was placed.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetOwnerOfVlogByReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // TODO Make single query - error checking is done by db fk
                var vlog = await _vlogRepository.GetVlogFromReactionAsync(reactionId).ConfigureAwait(false);
                var user = await _userRepository.GetUserFromVlogAsync(vlog.Id).ConfigureAwait(false);
                scope.Complete();
                return user;
            }
        }

        /// <summary>
        /// Gets a <see cref="Reaction"/> from our data store.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Reaction"/></returns>
        public Task<Reaction> GetReactionAsync(Guid reactionId)
        {
            return _reactionRepository.GetAsync(reactionId);
        }

        /// <summary>
        /// Gets the amount of <see cref="Reaction"/>s for a given <paramref name="vlogId"/>.
        /// </summary>
        /// <remarks>
        /// This throws a <see cref="EntityNotFoundException"/> if the <paramref name="vlogId"/>
        /// does not exist in our data store.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Reaction"/> count</returns>
        public async Task<int> GetReactionCountForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            if (!await _vlogRepository.ExistsAsync(vlogId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(vlogId)); }
            return await _reactionRepository.GetReactionCountForVlogAsync(vlogId).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all <see cref="Reaction"/> entities that belong to a given
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Reaction"/> collection</returns>
        public Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
        {
            return _reactionRepository.GetForVlogAsync(vlogId);
        }

        /// <summary>
        /// Called when we finish uploading a <see cref="Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnFinishedUploadingReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var reaction = await GetReactionAsync(reactionId).ConfigureAwait(false);
                if (reaction.ReactionState != ReactionState.Created) { throw new ReactionStateException($"Reaction not in {ReactionState.Created.GetEnumMemberAttribute()} state"); }

                // External checks
                await _amsClient.EnsureReactionTransformExistsAsync().ConfigureAwait(false);
                var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
                if (await amsClient.Assets.GetAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reaction.Id)).ConfigureAwait(false) == null)
                {
                    throw new ExternalErrorException($"Input asset for reaction with id {reaction.Id} does not exists in AMS");
                }
                // TODO Check - does asset have a video?

                // Internal operations
                await _reactionRepository.MarkProcessingAsync(reactionId).ConfigureAwait(false);

                // External operations
                var jobInput = new JobInputAsset(AMSNameGenerator.ReactionInputAssetName(reactionId),
                    start: new AbsoluteClipTime(TimeSpan.Zero),
                    end: new AbsoluteClipTime(TimeSpan.FromSeconds(swabbrConfiguration.ReactionLengthMaxInSeconds)));
                var jobOutputs = new JobOutput[] { new JobOutputAsset(AMSNameGenerator.ReactionOutputAssetName(reactionId)) };
                await amsClient.Jobs.CreateAsync(config.ResourceGroup, config.AccountName, AMSNameConstants.ReactionTransformName, AMSNameGenerator.ReactionJobName(reactionId), new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs
                }).ConfigureAwait(false);

                // Commit only if everything succeeded
                scope.Complete();
            }
        }

        /// <summary>
        /// Called when we finish transcoding a <see cref="Reaction"/> and failed.
        /// </summary>
        /// <remarks>
        /// This deletes all traces in AMS and in our database.
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnTranscodingReactionFailedAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var reaction = await GetReactionAsync(reactionId).ConfigureAwait(false);
                if (reaction.ReactionState != ReactionState.Processing) { throw new ReactionStateException($"Reaction not in {ReactionState.Processing.GetEnumMemberAttribute()} state"); }

                // External checks
                // TODO Implement

                // External operations
                await _storageService.CleanupReactionStorageOnFailureAsync(reactionId).ConfigureAwait(false);

                // Internal operations
                // TODO Is this correct?
                await _reactionRepository.MarkFailedAsync(reactionId).ConfigureAwait(false);
                await _reactionRepository.HardDeleteAsync(reactionId).ConfigureAwait(false);

                // TODO Notify some kind of failure to user?

                scope.Complete();
            }
        }

        /// <summary>
        /// Called when we finish transcoding a <see cref="Reaction"/> and succeeded.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnTranscodingReactionSucceededAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var reaction = await GetReactionAsync(reactionId).ConfigureAwait(false);
                if (reaction.ReactionState != ReactionState.Processing) { throw new ReactionStateException($"Reaction not in {ReactionState.Processing.GetEnumMemberAttribute()} state"); }

                // External checks
                // TODO Implement

                // External operations
                await _storageService.CleanupReactionStorageOnSuccessAsync(reactionId).ConfigureAwait(false);
                await _amsClient.CreateReactionStreamingLocatorAsync(reactionId).ConfigureAwait(false);

                // Internal operations
                await _reactionRepository.MarkFinishedAsync(reactionId).ConfigureAwait(false);
                await _notificationService.NotifyReactionPlacedAsync(reactionId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Reaction"/> in our data store and creates
        /// a new <see cref="Asset"/> in AMS for upload.
        /// </summary>
        /// <remarks>
        /// This is transactional, if the external creation process fails our
        /// internal creation will be rolled back.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="targetVlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="isPrivate">Indicates vlog private or not</param>
        /// <returns><see cref="ReactionUploadWrapper"/></returns>
        public async Task<ReactionUploadWrapper> PostReactionAsync(Guid userId, Guid targetVlogId, bool isPrivate)
        {
            userId.ThrowIfNullOrEmpty();
            targetVlogId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var vlog = await _vlogRepository.GetAsync(targetVlogId).ConfigureAwait(false); // Throws if marked deleted

                // Internal operations
                var reaction = await _reactionRepository.CreateAsync(new Reaction
                {
                    UserId = userId,
                    TargetVlogId = targetVlogId
                }).ConfigureAwait(false);

                // External checks
                var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
                if (await amsClient.Assets.GetAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reaction.Id)).ConfigureAwait(false) != null)
                {
                    throw new ExternalErrorException($"Input asset for reaction with id {reaction.Id} already exists in AMS");
                }

                // External operations
                await amsClient.Assets.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionInputAssetName(reaction.Id), new Asset()).ConfigureAwait(false);
                await amsClient.Assets.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, AMSNameGenerator.ReactionOutputAssetName(reaction.Id), new Asset()).ConfigureAwait(false);
                var sas = await amsClient.Assets.ListContainerSasAsync(config.ResourceGroup, config.AccountName,
                    AMSNameGenerator.ReactionInputAssetName(reaction.Id),
                    permissions: AssetContainerPermission.ReadWrite,
                    expiryTime: DateTime.UtcNow.AddMinutes(15).ToUniversalTime()).ConfigureAwait(false);

                //Commit, bundle and return
                scope.Complete();
                return new ReactionUploadWrapper
                {
                    Reaction = reaction,
                    UploadUrl = new Uri(sas.AssetContainerSasUrls.First())
                };
            }
        }

        /// <summary>
        /// Updates a <see cref="Reaction"/> in our data store.
        /// </summary>
        /// <remarks>
        /// TODO Clean this up
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <param name="isPrivate">If the reaction is private</param>
        /// <returns>The updated and queried <see cref="Reaction"/></returns>
        public async Task<Reaction> UpdateReactionAsync(Guid userId, Guid reactionId, bool isPrivate)
        {
            userId.ThrowIfNullOrEmpty();
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
                if (reaction.UserId != userId) { throw new NotAllowedException("User does not own reaction"); }

                reaction.IsPrivate = isPrivate;

                reaction = await _reactionRepository.UpdateAsync(reaction).ConfigureAwait(false);
                scope.Complete();
                return reaction;
            }
        }

    }

}
