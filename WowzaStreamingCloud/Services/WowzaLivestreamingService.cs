using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using static Swabbr.WowzaStreamingCloud.Services.WowzaLivestreamingServiceConstants;


namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Contains functionality for starting livestreams and handling the
    /// corresponding <see cref="Livestream"/> objects. This implementation
    /// uses Wowza.
    /// 
    /// TODO At the moment this doesn't check the user repository. I think that's ok, since we have a foreign key in the database.
    /// </summary>
    public sealed class WowzaLivestreamingService : ILivestreamingService, IDisposable
    {

        private readonly WowzaHttpClient _wowzaHttpClient;
        private readonly WowzaStreamingCloudConfiguration _wscOptions;
        private readonly ILivestreamRepository _livestreamRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public WowzaLivestreamingService(ILivestreamRepository livestreamRepository,
            IOptions<WowzaStreamingCloudConfiguration> wscOptions)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            if (wscOptions == null || wscOptions.Value == null) { throw new ArgumentNullException(nameof(wscOptions)); }
            _wscOptions = wscOptions.Value;
            _wowzaHttpClient = new WowzaHttpClient(_wscOptions);
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
        public async Task<Livestream> CreateAndStartLivestreamForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            // TODO Determine broadcast location based on user location?
            // TODO This has no error checking, maybe do a hard constraint check for this? Wowza won't tell us what's wrong...
            var request = new WscCreateLivestreamRequest()
            {
                Livestream = new WscCreateLiveStreamRequestBody
                {
                    AspectRatioWidth = _wscOptions.AspectRatioWidth,
                    AspectRatioHeight = _wscOptions.AspectRatioHeight,
                    BroadcastLocation = _wscOptions.BroadcastLocation,
                    Name = "Swabbr Livestream", // TODO Do we want to do anything with this?
                    BillingMode = BillingMode,
                    ClosedCaptionType = ClosedCaptionType,
                    DeliveryMethod = DeliveryMethod,
                    Encoder = Encoder,
                    PlayerType = PlayerType,
                    TranscoderType = TranscoderType,
                    HostedPage = HostedPage,
                    HostedPageSharingIcons = HostedPageSharingIcons,
                    LowLatency = LowLatency,
                    PlayerResponsive = PlayerResponsive,
                    Recording = Recording,
                }
            };

            try
            {
                // TODO Transaction?
                // TODO Put all other Wowza-specific properties in a json file here to store in the db!
                // First create a livstream
                var response = await _wowzaHttpClient.CreateLivestream(request).ConfigureAwait(false);
                var livestream = await _livestreamRepository.CreateAsync(new Livestream
                {
                    ExternalId = response.Livestream.Id,
                    IsActive = false,
                    BroadcastLocation = response.Livestream.BroadcastLocation,
                    CreateDate = response.Livestream.CreatedAt,
                    Name = response.Livestream.Name,
                    UserId = userId,
                    LivestreamStatus = LivestreamStatus.Created
                }).ConfigureAwait(false);

                // Now start the livestream, update status afterwards.
                await _wowzaHttpClient.StartLivestreamAsync(livestream.ExternalId).ConfigureAwait(false);
                await _livestreamRepository.UpdateLivestreamStatusAsync(livestream.Id, LivestreamStatus.PendingUser).ConfigureAwait(false);

                return livestream;
            }
            catch (HttpRequestException e)
            {
                // TODO Specify based on status code?
                // 409 --> Limited Resources: Maximum number of streams we can create in x-hour period has been reached
                // 422 --> Unprocessable entity: You've reached the maximum number of transcoded (ABR) transcoders that may be started at one time during a trial
                throw new ExternalErrorException("Could not create a new WSC livestream", e);
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
                if (livestream.UserId != userId) { throw new InvalidOperationException("Given user doesn't own livestream"); }
                if (livestream.LivestreamStatus != LivestreamStatus.PendingUser) { throw new InvalidOperationException("Livestream must be marked as pending_user before going live"); }

                // Check if we aren't already streaming
                if (await _wowzaHttpClient.IsStreamerConnected(livestream.ExternalId)) { throw new InvalidOperationException("A user already connected to the livestream in the Wowza cloud"); }

                // Throw if we already have a livestream recording on Wowza
                // if (await _wowzaHttpClient.DoesLivestreamHaveRecording(livestream.ExternalId)) { throw new InvalidOperationException("User has already streamed"); }
                // TODO Race condition
                int i = 0;

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

                // Check if Wowza actually has a recording, so we know if a stream has occurred before
                //if (!await _wowzaHttpClient.DoesLivestreamHaveRecording(livestream.ExternalId)) { throw new InvalidOperationException("User hasn't streamed content yet"); }
                // TODO Race condition
                int i = 0;

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
        /// Deletes a livestream from the Wowza platform.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task DiscardLivestreamAsync(Guid livestreamId)
        {
            throw new NotImplementedException("We aren't sure yet if we ever need to throw away livestreams");
        }

        /// <summary>
        /// Called on graceful shutdown, see <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            _wowzaHttpClient.Dispose();
        }

    }

}
