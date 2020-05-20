using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Handles <see cref="Livestream"/> pool functionality.
    /// </summary>
    public sealed class AMSLivestreamPoolService : ILivestreamPoolService
    {

        private readonly ILivestreamRepository _livestreamRepository;
        private readonly AMSConfiguration _config;
        private readonly IAMSClient _amsClient;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamPoolService(ILivestreamRepository livestreamRepository,
            IOptions<AMSConfiguration> config,
            IAMSClient amsClient)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));

            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
        }

        /// <summary>
        /// Cleans up a <see cref="Livestream"/> <see cref="LiveEvent"/> for 
        /// re-usage, both internally and externally. This should be used after
        /// a graceful <see cref="Livestream"/> and <see cref="Vlog"/> process.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamState != LivestreamState.PendingClosure) { throw new LivestreamStateException($"Livestream not in {LivestreamState.PendingClosure.GetEnumMemberAttribute()} state"); }

                // External checks
                // TODO Implement

                // External operations
                // TODO What do we need to do here?

                // Internal operations
                // TODO This just resets the livestream, check this
                await _livestreamRepository.MarkClosedAsync(livestream.Id).ConfigureAwait(false);
                await _livestreamRepository.MarkCreatedAsync(livestream.Id, livestream.ExternalId, livestream.BroadcastLocation).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Cleans up a livestream in the <see cref="LivestreamState.UserNeverConnectedTimeout"/>
        /// state.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupNeverConnectedLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamState != LivestreamState.UserNeverConnectedTimeout) { throw new LivestreamStateException($"Livestream not in {LivestreamState.UserNeverConnectedTimeout.GetEnumMemberAttribute()} state"); }

                // External checks
                // TODO Implement

                // External operations
                // TODO What do we need to do here?

                // Internal operations
                // TODO This just resets the livestream, check this
                await _livestreamRepository.MarkCreatedAsync(livestream.Id, livestream.ExternalId, livestream.BroadcastLocation).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Cleans up a <see cref="Livestream"/> <see cref="LiveEvent"/> for 
        /// re-usage, both internally and externally. This should be called after
        /// thie <see cref="SwabbrUser"/> vlog request timed out.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task CleanupTimedOutLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Internal checks
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamState != LivestreamState.UserNoResponseTimeout) { throw new LivestreamStateException($"Livestream not in {LivestreamState.UserNoResponseTimeout.GetEnumMemberAttribute()} state"); }

                // External checks
                // TODO Implement

                // External operations
                // TODO What do we need to do here?

                // Internal operations
                // TODO This just resets the livestream, check this
                await _livestreamRepository.MarkPendingClosureAsync(livestream.Id).ConfigureAwait(false);
                await _livestreamRepository.MarkClosedAsync(livestream.Id).ConfigureAwait(false);
                await _livestreamRepository.MarkCreatedAsync(livestream.Id, livestream.ExternalId, livestream.BroadcastLocation).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Creates and sets up a new <see cref="Livestream"/> in Azure Media
        /// Services.
        /// </summary>
        /// <remarks>
        /// TODO Edge case checking & reporting --> max live events per account in AMS
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> CreateLivestreamAsync()
        {
            // First create internally (state will be created_internal)
            var livestream = await _livestreamRepository.CreateAsync(new Livestream
            {
                Name = AMSNameConstants.LivestreamDefaultName
            }).ConfigureAwait(false);

            // Create externally
            await _amsClient.EnsureLivestreamTransformExistsAsync().ConfigureAwait(false);
            await _amsClient.EnsureStreamingEndpointRunningAsync().ConfigureAwait(false);
            var liveEvent = await _amsClient.CreateLiveEventAsync(livestream.Id).ConfigureAwait(false);

            // Update internally
            livestream.ExternalId = liveEvent.Name;
            livestream.BroadcastLocation = liveEvent.Location;
            await _livestreamRepository.MarkCreatedAsync(livestream.Id, livestream.ExternalId, livestream.BroadcastLocation).ConfigureAwait(false);

            return await _livestreamRepository.GetAsync(livestream.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts to get a <see cref="Livestream"/> from the existing pool.
        /// </summary>
        /// <remarks>
        /// This does NOT claim the <see cref="Livestream"/>. Do this using a
        /// transaction (<see cref="TransactionScope"/> is an option).
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> TryGetLivestreamFromPoolAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestreams = await _livestreamRepository.GetAvailableLivestreamsAsync().ConfigureAwait(false);
                if (livestreams.Any())
                {
                    var livestream = livestreams.First();
                    scope.Complete();
                    return livestream;
                }
            }

            return await CreateLivestreamAsync().ConfigureAwait(false);
        }

    }

}
