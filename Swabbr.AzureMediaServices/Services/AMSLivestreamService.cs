﻿using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Streaming service based on Azure Media Services.
    /// </summary>
    public sealed class AMSLivestreamService : ILivestreamService
    {

        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamPoolService _livestreamPoolService;
        private readonly IAMSClient _amsClient;
        private readonly IVlogService _vlogService;
        private readonly IStorageService _storageService;
        private readonly SwabbrConfiguration _swabbrConfiguration;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamService(ILivestreamRepository livestreamRepository,
            ILivestreamPoolService livestreamPoolService,
            IAMSClient amsClient,
            IVlogService vlogService,
            IStorageService storageService,
            IOptions<SwabbrConfiguration> options)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _livestreamPoolService = livestreamPoolService ?? throw new ArgumentNullException(nameof(livestreamPoolService));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));

            if (options == null || options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            _swabbrConfiguration = options.Value;
            _swabbrConfiguration.ThrowIfInvalid();
        }

        /// <summary>
        /// Gets a <see cref="Livestream"/> by its external id.
        /// </summary>
        /// <param name="externalId">External <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream"/></returns>
        public Task<Livestream> GetLivestreamFromExternalIdAsync(string externalId)
        {
            return _livestreamRepository.GetByExternalIdAsync(externalId);
        }

        /// <summary>
        /// Gets a <see cref="Livestream"/> based on the <see cref="SwabbrUser"/>
        /// and the minute in which the request was triggered.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="triggerMinute">Trigger minute</param>
        /// <returns><see cref="Livestream"/></returns>
        public Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute)
        {
            return _livestreamRepository.GetLivestreamFromTriggerMinute(userId, triggerMinute);
        }

        /// <summary>
        /// Generates <see cref="ParametersRecordVlog"/>.
        /// </summary>
        /// <remarks>
        /// TODO Maybe make this check if the user and livestream match? User isn't
        /// being used at all right now.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="triggerMinute">User trigger minute</param>
        /// <returns><see cref="ParametersRecordVlog"/></returns>
        public async Task<ParametersRecordVlog> GetParametersRecordVlogAsync(Guid livestreamId, DateTimeOffset triggerMinute)
        {
            livestreamId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }

            return new ParametersRecordVlog
            {
                LivestreamId = livestreamId,
                VlogId = (await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false)).Id,
                RequestMoment = triggerMinute.ToTriggerMinute(),
                RequestTimeout = TimeSpan.FromMinutes(_swabbrConfiguration.VlogRequestTimeoutMinutes)
            };
        }

        /// <summary>
        /// Gets the upstream details for a <see cref="Livestream"/>.
        /// </summary>
        /// <remarks>
        /// TODO Link this with <see cref="OnUserStartStreamingAsync(Guid, Guid)"/>?
        /// That function has all the information required, but might be refactored.
        /// This would kind-of be AMS specific.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="LivestreamUpstreamDetails"/></returns>
        public async Task<LivestreamUpstreamDetails> GetUpstreamDetailsAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUserConnect) { throw new LivestreamStateException(LivestreamStatus.PendingUserConnect.GetEnumMemberAttribute()); }
                if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }
                // TODO User trigger minute validation?

                var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

                scope.Complete();
                return await _amsClient.GetUpstreamDetailsAsync(livestreamId, vlog.Id, livestream.ExternalId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Called when the user connects to the actual <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserConnectedToLivestreamAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId);
                if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUserConnect) { throw new LivestreamStateException($"Livestream not in {LivestreamStatus.PendingUserConnect.GetEnumMemberAttribute()} state"); }

                var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);
                if (vlog.UserId != userId) { throw new UserNotOwnerException(nameof(Vlog)); }

                // External checks
                // TODO Implement

                // Internal operations
                await _livestreamRepository.MarkLiveAsync(livestreamId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Called when the user disconnects from the actual <see cref="Livestream"/>.
        /// </summary>
        /// <remarks>
        /// TODO Do we need this?
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserDisconnectedFromLivestreamAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId);
                if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }
                if (livestream.LivestreamStatus != LivestreamStatus.Live) { throw new LivestreamStateException($"Livestream not in {LivestreamStatus.Live.GetEnumMemberAttribute()} state"); }

                var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);
                if (vlog.UserId != userId) { throw new UserNotOwnerException(nameof(Vlog)); }

                // External checks
                // TODO Implement

                // External operations
                await _amsClient.StopLiveOutputAsync(vlog.Id, livestream.ExternalId).ConfigureAwait(false);
                await _amsClient.StopLiveEventAsync(livestream.ExternalId).ConfigureAwait(false);

                // Internal operations
                await _livestreamRepository.MarkPendingClosureAsync(livestreamId).ConfigureAwait(false);

                scope.Complete();
            }

            // TODO Is this correct, outside tscope from this class?
            await _livestreamPoolService.CleanupLivestreamAsync(livestreamId).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when a <see cref="SwabbrUser"/> notifies that he or she will
        /// start streaming.
        /// </summary>
        /// <remarks>
        /// A <see cref="Vlog"/> must already exist here.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserStartStreamingAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUser) { throw new LivestreamStateException(livestream.LivestreamStatus.GetEnumMemberAttribute()); }

                var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);
                if (vlog.UserId != userId) { throw new UserNotOwnerException(nameof(Vlog)); }

                // External checks
                // TODO Implement

                // External operations
                await _amsClient.StartLiveEventAsync(livestream.ExternalId).ConfigureAwait(false);
                await _amsClient.CreateLiveOutputAsync(vlog.Id, livestream.ExternalId).ConfigureAwait(false);
                await _amsClient.CreateLivestreamVlogStreamingLocatorAsync(vlog.Id, livestream.ExternalId).ConfigureAwait(false);

                // Internal operations
                await _livestreamRepository.MarkPendingUserConnectAsync(livestreamId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Processes the operation after a <see cref="SwabbrUser"/> notified the
        /// backend that he or she stopped streaming.
        /// </summary>
        /// <remarks>
        /// TODO At the moment this is obsolete.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/>id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // At the moment this does nothing

                scope.Complete();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Processes a <see cref="Livestream"/> timeout.
        /// </summary>
        /// <remarks>
        /// This function assumes that when called, a timeout actually is required.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessTimeoutAsync(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUser) { throw new LivestreamStateException($"Livestream not in {LivestreamStatus.PendingUser.GetEnumMemberAttribute()} during timeout processing"); }
                if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }

                // External checks
                // TODO Implement

                // External operations
                await _amsClient.StopLiveEventAsync(livestream.ExternalId).ConfigureAwait(false);
                // TODO Cleanup streaming asset

                // Internal operations
                var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);
                await _livestreamRepository.MarkUserNoResponseTimeoutAsync(livestreamId).ConfigureAwait(false); // This deletes the vlog as well
                await _storageService.CleanupVlogStorageOnDeleteAsync(vlog.Id).ConfigureAwait(false);

                // TODO Should this function be responsible for the cleanup operation call?
                // Clean up livestream for reusage
                await _livestreamPoolService.CleanupTimedOutLivestreamAsync(livestreamId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Attempts to setup and start a <see cref="Livestream"/> for a 
        /// <see cref="SwabbrUser"/>.
        /// </summary>
        /// <remarks>
        /// TODO The GUID assignment is done outside of the tscope. If we poll the
        /// livestream within the tscope, we get an error that the scope is already 
        /// complete.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="triggerMinute">Trigger minute</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> TryClaimLivestreamForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            userId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); } // TODO DTO is not nullable

            var livestream = await _livestreamPoolService.TryGetLivestreamFromPoolAsync().ConfigureAwait(false);
            if (livestream.LivestreamStatus != LivestreamStatus.Created) { throw new LivestreamStateException(livestream.LivestreamStatus.GetEnumMemberAttribute()); }

            await _livestreamRepository.MarkPendingUserAsync(livestream.Id, userId, triggerMinute).ConfigureAwait(false);

            return await _livestreamRepository.GetAsync(livestream.Id).ConfigureAwait(false);
        }

    }

}
