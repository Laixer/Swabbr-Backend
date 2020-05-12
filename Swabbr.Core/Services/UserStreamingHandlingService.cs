using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// This processes the requests that a <see cref="SwabbrUser"/> sends to our
    /// backend when he or she starts or stops livestreaming (or is about to).
    /// </summary>
    public sealed class UserStreamingHandlingService : IUserStreamingHandlingService
    {

        private readonly ILivestreamService _livestreamingService;
        private readonly ILivestreamPlaybackService _livestreamPlaybackService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogService _vlogService;
        private readonly ILogger logger;
        private readonly LogicAppsConfiguration logicAppsConfiguration;
        private readonly SwabbrConfiguration swabbrConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserStreamingHandlingService(ILivestreamService livestreamingService,
            ILivestreamPlaybackService livestreamPlaybackService,
            INotificationService notificationService,
            IUserRepository userRepository,
            ILivestreamRepository livestreamRepository,
            IVlogService vlogService,
            ILoggerFactory loggerFactory,
            IOptions<LogicAppsConfiguration> optionsLogicApps,
            IOptions<SwabbrConfiguration> optionsSwabbr,
            IHttpClientFactory httpClientFactory)
        {
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(UserStreamingHandlingService)) : throw new ArgumentNullException(nameof(loggerFactory));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            if (optionsLogicApps == null || optionsLogicApps.Value == null) { throw new ArgumentNullException(nameof(optionsLogicApps)); }
            logicAppsConfiguration = optionsLogicApps.Value;
            logicAppsConfiguration.ThrowIfInvalid();

            if (optionsSwabbr == null || optionsSwabbr.Value == null) { throw new ArgumentNullException(nameof(optionsSwabbr)); }
            swabbrConfiguration = optionsSwabbr.Value;
            swabbrConfiguration.ThrowIfInvalid();
        }

        /// <summary>
        /// Called when the user connected to the <see cref="Livestream"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserConnectedToLivestreamAsync(Guid userId, Guid livestreamId)
        {
            try
            {
                logger.LogTrace($"{nameof(OnUserConnectedToLivestreamAsync)} - Processing event where user {userId} connected to livestream {livestreamId}");
                livestreamId.ThrowIfNullOrEmpty();

                await _livestreamingService.OnUserConnectedToLivestreamAsync(livestreamId, userId).ConfigureAwait(false);

                // Notify all followers
                await _notificationService.NotifyFollowersProfileLiveAsync(userId, livestreamId, new ParametersFollowedProfileLive
                {
                    LiveLivestreamId = livestreamId,
                    LiveUserId = userId,
                    LiveVlogId = (await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false)).Id
                }).ConfigureAwait(false);

                logger.LogTrace($"{nameof(OnUserConnectedToLivestreamAsync)} - Finished processing event where user {userId} connected to livestream {livestreamId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(OnUserConnectedToLivestreamAsync)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Called when the user disconnected from the <see cref="Livestream"/>.
        /// </summary>
        /// <remarks>
        /// This does nothing if the livestream is not in state <see cref="LivestreamStatus.Live"/>.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserDisconnectedFromLivestreamAsync(Guid userId, Guid livestreamId)
        {
            try
            {
                logger.LogTrace($"{nameof(OnUserDisconnectedFromLivestreamAsync)} - Processing event where user {userId} disconnected from livestream {livestreamId}");
                livestreamId.ThrowIfNullOrEmpty();

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                    if (livestream.LivestreamStatus != LivestreamStatus.Live) { return; }

                    // Grab the vlog, this gets demarked in the livestream processing functions
                    // var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

                    // First process the livestream
                    await _livestreamingService.OnUserDisconnectedFromLivestreamAsync(livestreamId, userId).ConfigureAwait(false);

                    scope.Complete();
                }
                // Then process any notifications (not a requirement at this moment)
                // TODO Re-enable
                // await _notificationService.NotifyFollowersVlogPostedAsync(userId, vlog.Id).ConfigureAwait(false);

                logger.LogTrace($"{nameof(OnUserDisconnectedFromLivestreamAsync)} - Finished processing event where user {userId} disconnected from livestream {livestreamId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(OnUserDisconnectedFromLivestreamAsync)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Fires after the timeout for user connecting, after the user told the
        /// backend he would start streaming.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserNeverConnectedCheckAsync(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

                // Only do something if we are in the pending user connect state
                if (livestream.LivestreamStatus == LivestreamStatus.PendingUserConnect)
                {
                    await _livestreamingService.OnUserNeverConnectedToLivestreamAsync(livestreamId, userId).ConfigureAwait(false);
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// Called when the user starts streaming to a <see cref="Livestream"/>.
        /// This creates a <see cref="Vlog"/> to bind to the stream.
        /// </summary>
        /// <remarks>
        /// This should be called BEFORE the actual streaming itself.
        /// This does NOT notify us if a <see cref="SwabbrUser"/> doesn't exist,
        /// this relation exists implicitly.
        /// 
        /// TODO This is coupled with Azure Logic Apps.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="LivestreamUpstreamDetails"/></returns>
        public async Task<LivestreamUpstreamDetails> OnUserStartStreaming(Guid userId, Guid livestreamId)
        {
            try
            {
                logger.LogTrace($"{nameof(OnUserStartStreaming)} - Attempting to start livestream {livestreamId} for user {userId}");
                userId.ThrowIfNullOrEmpty();
                livestreamId.ThrowIfNullOrEmpty();

                await _livestreamingService.OnUserStartStreamingAsync(livestreamId, userId).ConfigureAwait(false); // DB logic creates vlog

                var upstreamDetails = await _livestreamingService.GetUpstreamDetailsAsync(livestreamId, userId).ConfigureAwait(false);

                logger.LogTrace($@"{nameof(OnUserStartStreaming)} - Successfully started livestream {livestreamId} for user {userId},
                    triggering logic app user connect timeout function");

                // Trigger logic app
                var client = _httpClientFactory.GetClient();
                var json = JsonConvert.SerializeObject(new UserStartStreamingWrapper
                {
                    LivestreamId = livestreamId,
                    UserId = userId,
                    UserConnectTimeoutSeconds = swabbrConfiguration.UserConnectTimeoutSeconds
                });
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(logicAppsConfiguration.EndpointUserConnectTimeout, content).ConfigureAwait(false);
                
                if (!result.IsSuccessStatusCode)
                {
                    logger.LogError($@"{nameof(OnUserStartStreaming)} - Failed triggering user connect logic app for livestream {livestreamId} for user {userId} -
                        {result.StatusCode} {result.ReasonPhrase}");
                }
                else
                {
                    logger.LogTrace($"{nameof(OnUserStartStreaming)} - Successfully triggered user connect logic app for livestream {livestreamId} for user {userId}");
                }
                return upstreamDetails;
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(OnUserStartStreaming)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets called when the user stops streaming.
        /// </summary>
        /// <remarks>
        /// This should be called AFTER the user has stopped streaming.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        [Obsolete("Does nothing")]
        public async Task OnUserStopStreaming(Guid userId, Guid livestreamId)
        {
            try
            {
                logger.LogTrace($"{nameof(OnUserStopStreaming)} - Attempting to stop livestream {livestreamId} for user {userId}");
                userId.ThrowIfNullOrEmpty();
                livestreamId.ThrowIfNullOrEmpty();

                await _livestreamingService.OnUserStopStreamingAsync(livestreamId, userId).ConfigureAwait(false);

                logger.LogTrace($"{nameof(OnUserStopStreaming)} - Successfully stopped livestream {livestreamId} for user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(OnUserStopStreaming)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Called when the user vlog time has expired.
        /// </summary>
        /// <remarks>
        /// This gets called even when the user has alreayd stopped vlogging.
        /// In this case, the function will just exit gracefully. This will only
        /// process if the livestream is in state <see cref="LivestreamStatus.PendingUserConnect"/>.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserVlogTimeExpiredAsync(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
                if (livestream.LivestreamStatus == LivestreamStatus.Live)
                {
                    if (livestream.UserId != userId) { throw new UserNotOwnerException(nameof(Livestream)); }
                    await _livestreamingService.OnUserVlogTimeExpiredAsync(livestreamId, userId).ConfigureAwait(false);
                }
                scope.Complete();
            }
        }
    }

}
