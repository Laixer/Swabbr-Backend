using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities.Livestreams;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Contains functionality for starting livestreams and handling the
    /// corresponding <see cref="Livestream"/> objects. This implementation
    /// uses Wowza.
    /// 
    /// TODO At the moment this doesn't check the user repository. I think that's ok, since we have a foreign key in the database.
    /// </summary>
    public sealed class WowzaLivestreamService : ILivestreamService, IDisposable
    {

        private readonly WowzaHttpClient _wowzaHttpClient;
        private readonly WowzaStreamingCloudConfiguration _wscOptions;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamPoolService _livestreamPoolService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public WowzaLivestreamService(ILivestreamRepository livestreamRepository,
            IOptions<WowzaStreamingCloudConfiguration> wscOptions,
            ILivestreamPoolService livestreamPoolService)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            if (wscOptions == null || wscOptions.Value == null) { throw new ArgumentNullException(nameof(wscOptions)); }
            _wscOptions = wscOptions.Value;
            _wowzaHttpClient = new WowzaHttpClient(_wscOptions);
            _livestreamPoolService = livestreamPoolService ?? throw new ArgumentNullException(nameof(livestreamPoolService));
        }

        /// <summary>
        /// Creates a new <see cref="Livestream"/> for a given user.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="Livestream"/> is already up and running in
        /// Wowza when we return it. This might take a while, since Wowza needs
        /// some time to setup things (30sec - 2min).
        /// </remarks>
        /// <param name="userId">Internal user id</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> TryStartLivestreamForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            // TODO Determine broadcast location based on user location?
            try
            {
                var livestream = await _livestreamPoolService.TryGetLivestreamFromPoolAsync();

                // Now start the livestream, update status afterwards.
                await _wowzaHttpClient.StartLivestreamAsync(livestream.ExternalId).ConfigureAwait(false);
                await _livestreamRepository.MarkPendingUserAsync(livestream.Id, userId).ConfigureAwait(false);

                return livestream;
            }
            catch (HttpRequestException e)
            {
                // TODO Specify based on status code?
                // 409 --> Limited Resources: Maximum number of streams we can create in x-hour period has been reached
                // 422 --> Unprocessable entity: Seems like invalid json request body
                throw new ExternalErrorException("Could not create a new WSC livestream", e);
            }
            catch (NoLivestreamAvailableException)
            {
                throw; // Good as is, just throw upwards
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not create a new WSC livestream", e);
            }
        }

        /// <summary>
        /// Marks the <see cref="Livestream"/> as live. This has to be called 
        /// as soon as the user starts streaming.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="InvalidOperationException"/> if the 
        /// <see cref="Livestream.LivestreamStatus"/> property isn't set to 
        /// <see cref="LivestreamStatus.PendingUser"/> in our data store.
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
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false); // For update
                if (livestream.UserId != userId) { throw new UserNotOwnerException(); }
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUser) { throw new LivestreamStateException(); }

                // Throw if the Wowza livestream isn't actually live
                if (!await _wowzaHttpClient.IsLivestreamStartedAsync(livestream.ExternalId).ConfigureAwait(false))
                {
                    // TODO Specify this exception more
                    throw new InvalidOperationException("Livestream isn't in the started state on Wowza");
                }

                // Check if we aren't already streaming
                if (await _wowzaHttpClient.IsStreamerConnected(livestream.ExternalId)) { throw new InvalidOperationException("A user already connected to the livestream in the Wowza cloud"); }

                await _livestreamRepository.UpdateLivestreamStatusAsync(livestream.Id, LivestreamStatus.Live).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Stops an existing <see cref="Livestream"/> in the Swabbr cloud.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                livestream.ExternalId.ThrowIfNullOrEmpty();
                livestream.UserId.ThrowIfNullOrEmpty();
                if (livestream.UserId != userId) { throw new InvalidOperationException("User doesn't own livestream"); }
                if (livestream.LivestreamStatus != LivestreamStatus.Live) { throw new InvalidOperationException("Can't close a non-live livestream"); }

                // Check if Wowza actually has someone that's connected
                if (await _wowzaHttpClient.IsStreamerConnected(livestream.ExternalId)) { throw new InvalidOperationException("User still connected to the livestream in the Wowza cloud"); }

                // First set the livestream to pending closure
                await _livestreamRepository.UpdateLivestreamStatusAsync(livestream.Id, LivestreamStatus.PendingClosure).ConfigureAwait(false);

                // Now stop it externally as well, then notify the database
                await _wowzaHttpClient.StopLivestreamAsync(livestream.ExternalId);
                await _livestreamRepository.UpdateLivestreamStatusAsync(livestream.Id, LivestreamStatus.Closed).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Called when the backend determines it's time to abort the user livestream 
        /// request.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task AbortLivestreamAsync(Guid livestreamId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called on graceful shutdown, see <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            _wowzaHttpClient.Dispose();
        }

        /// <summary>
        /// Checks if a <see cref="Livestream"/> is live and belongs to the given
        /// <paramref name="userId"/>. This can be called to validate before pushing
        /// livestream notifications to followers.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns></returns>
        public async Task<bool> IsLivestreamValidForFollowersAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
            if (livestream.UserId != userId) { throw new UserNotOwnerException(); }

            if (!await _wowzaHttpClient.IsLivestreamStreamingAsync(livestream.ExternalId).ConfigureAwait(false))
            {
                return false;
            }

            // If we reach this we have passed all our checks
            return true;
        }

        /// <summary>
        /// Extracts all parameters we need to start streaming/
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="ParametersRecordVlog"/></returns>
        public async Task<ParametersRecordVlog> GetUpstreamParametersAsync(Guid livestreamId, Guid userId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
            if (livestream.UserId != userId) { throw new UserNotOwnerException(); }

            var wscLivestream = await _wowzaHttpClient.GetLivestreamAsync(livestream.ExternalId).ConfigureAwait(false);
            ValidateWscLivestream(wscLivestream);

            // Construct parameters
            return new ParametersRecordVlog
            {
                LivestreamId = livestreamId,
                ApplicationName = wscLivestream.Livestream.SourceConnectionInformation.Application,
                HostPort = wscLivestream.Livestream.SourceConnectionInformation.HostPort,
                HostServer = wscLivestream.Livestream.SourceConnectionInformation.PrimaryServer,
                Password = wscLivestream.Livestream.SourceConnectionInformation.Password,
                StreamKey = wscLivestream.Livestream.SourceConnectionInformation.StreamName,
                Username = wscLivestream.Livestream.SourceConnectionInformation.Username
            };
        }

        /// <summary>
        /// Throws if any of the <see cref="WscLivestreamResponse"/> properties 
        /// are null, empty or invalid.
        /// </summary>
        /// <param name="wscLivestreamResponse"><see cref="WscLivestreamResponse"/></param>
        private void ValidateWscLivestream(WscLivestreamResponse wscLivestreamResponse) {
            if (wscLivestreamResponse == null) { throw new ArgumentNullException(nameof(wscLivestreamResponse)); }
            if (wscLivestreamResponse.Livestream == null) { throw new ArgumentNullException(nameof(wscLivestreamResponse.Livestream)); }
            if (wscLivestreamResponse.Livestream.Id == null) { throw new ArgumentNullException(nameof(wscLivestreamResponse.Livestream.Id)); }

            var connection = wscLivestreamResponse.Livestream.SourceConnectionInformation ?? throw new ArgumentNullException(nameof(wscLivestreamResponse.Livestream.SourceConnectionInformation));
            connection.Application.ThrowIfNullOrEmpty();
            if (connection.HostPort == 0) { throw new ArgumentNullException(nameof(connection.HostPort)); }
            connection.Password.ThrowIfNullOrEmpty();
            if (connection.PrimaryServer == null) { throw new ArgumentNullException(nameof(connection.PrimaryServer)); }
            connection.StreamName.ThrowIfNullOrEmpty();
            connection.Username.ThrowIfNullOrEmpty();
        }

    }

}
