using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Contains functionality to handle notification operations.
    /// </summary>
    public sealed class NotificationService : INotificationService
    {

        private readonly IUserRepository _userRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly IReactionService _reactionService;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamService _livestreamingService;
        private readonly INotificationClient _notificationClient;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationService(IUserRepository userRepository,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IReactionService reactionService,
            ILivestreamRepository livestreamRepository,
            ILivestreamService livestreamingService,
            INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(NotificationService)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Sends a <see cref="SwabbrNotification"/> to each follower of the 
        /// specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotifyFollowersProfileLiveAsync(Guid userId, Guid livestreamId, ParametersFollowedProfileLive pars)
        {
            try
            {
                logger.LogTrace($"{nameof(NotifyFollowersProfileLiveAsync)} - Attempting notifying followers for livestream {livestreamId} from user {userId}");
                userId.ThrowIfNullOrEmpty();
                livestreamId.ThrowIfNullOrEmpty();

                if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }
                if (await _livestreamingService.IsLivestreamValidForFollowersAsync(livestreamId, userId).ConfigureAwait(false))
                {
                    throw new InvalidOperationException("First contact backend, then start streaming!");
                }

                var notification = new SwabbrNotification(NotificationAction.FollowedProfileLive)
                {
                    Body = "TODO CENTRALIZE Your following user profile is live!",
                    Title = "TODO CENTRALIZE User is live!",
                    Pars = pars
                };

                var pushDetails = await _userRepository.GetFollowersPushDetailsAsync(userId).ConfigureAwait(false);

                // TODO Future optimization
                // var detailsAndroid = new List<UserPushNotificationDetails>();
                // var detailsIos = new List<UserPushNotificationDetails>();

                foreach (var item in pushDetails)
                {
                    await _notificationClient.SendNotificationAsync(item.UserId, item.PushNotificationPlatform, notification).ConfigureAwait(false);
                    logger.LogTrace($"{nameof(NotifyFollowersProfileLiveAsync)} - Notified user {item.UserId}");
                }
                logger.LogTrace($"{nameof(NotifyFollowersProfileLiveAsync)} - Completed notifying followers for livestream {livestreamId} from user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyFollowersProfileLiveAsync)} - Exception in method ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends a notification to a <see cref="SwabbrUser"/> when we have decided
        /// that the user should start streaming.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/>id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotifyVlogRecordRequestAsync(Guid userId, Guid livestreamId, ParametersRecordVlog pars)
        {
            try
            {
                logger.LogTrace($"{nameof(NotifyVlogRecordRequestAsync)} - Attempting vlog record request for livestream {livestreamId} to user {userId}");
                userId.ThrowIfNullOrEmpty();
                livestreamId.ThrowIfNullOrEmpty();
                pars.Validate();

                if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }

                var notification = new SwabbrNotification(NotificationAction.VlogRecordRequest)
                {
                    Body = "TODO CENTRALIZE A livestream is ready to stream whatever you want!",
                    Title = "TODO CENTRALIZE Time to record a vlog!",
                    Pars = pars
                };

                var pushDetails = await _userRepository.GetPushDetailsAsync(userId).ConfigureAwait(false);
                await _notificationClient.SendNotificationAsync(pushDetails.UserId, pushDetails.PushNotificationPlatform, notification).ConfigureAwait(false);
                logger.LogTrace($"{nameof(NotifyVlogRecordRequestAsync)} - Completed vlog record request for livestream {livestreamId} to user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyVlogRecordRequestAsync)} - Exception in method ", e.Message);
                throw;
            }
        }

        public Task NotifyVlogRecordTimeoutAsync(Guid userId)
        {
            logger.LogError($"Implement {nameof(NotifyVlogRecordTimeoutAsync)} - called for user {userId}");
            throw new NotImplementedException(nameof(NotifyVlogRecordTimeoutAsync));
        }

        /// <summary>
        /// Notifies the owner of the vlog to which a reaction is placed that
        /// a reaction was placed.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotifyReactionPlacedAsync(Guid reactionId)
        {
            try
            {
                logger.LogTrace($"{nameof(NotifyReactionPlacedAsync)} - Attempting vlog reaction notification for reaction {reactionId}");

                reactionId.ThrowIfNullOrEmpty();

                // TODO Is this the right way to do this? Maybe some other service should handle the details extraction
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var vlog = await _vlogRepository.GetVlogFromReactionAsync(reactionId).ConfigureAwait(false);
                    var user = await _userRepository.GetUserFromVlogAsync(vlog.Id).ConfigureAwait(false);
                    var userPushDetails = await _userRepository.GetPushDetailsAsync(user.Id).ConfigureAwait(false);

                    var pars = new ParametersVlogNewReaction
                    {
                        ReactionId = reactionId,
                        VlogId = vlog.Id
                    };
                    var notification = new SwabbrNotification(NotificationAction.VlogNewReaction)
                    {
                        Body = "TODO CENTRALIZE",
                        Title = "TODO CENTRALIZE",
                        Pars = pars
                    };

                    // First release, then push
                    scope.Complete();
                    await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform, notification).ConfigureAwait(false);
                }

                logger.LogTrace($"{nameof(NotifyReactionPlacedAsync)} - Attempting vlog reaction notification for reaction {reactionId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyReactionPlacedAsync)} - Exception in method ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Notifies the owner of a <see cref="Vlog"/> when someone likes the 
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotificationVlogLikedAsync(VlogLikeId vlogLikeId)
        {
            try
            {
                logger.LogTrace($"{nameof(NotificationVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");

                if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
                vlogLikeId.UserId.ThrowIfNullOrEmpty();
                vlogLikeId.VlogId.ThrowIfNullOrEmpty();

                // TODO Is this the right way to do this? Maybe some other service should handle the details extraction
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // We don't need to check for vlog or userthatliked existance - they are foreign keys already
                    var vlogLike = await _vlogLikeRepository.GetAsync(vlogLikeId).ConfigureAwait(false);
                    var user = await _userRepository.GetUserFromVlogAsync(vlogLikeId.VlogId).ConfigureAwait(false);
                    var userPushDetails = await _userRepository.GetPushDetailsAsync(user.Id).ConfigureAwait(false);

                    var pars = new ParametersVlogGainedLike
                    {
                        UserThatLikedId = vlogLikeId.UserId,
                        VlogId = vlogLikeId.VlogId
                    };
                    var notification = new SwabbrNotification(NotificationAction.VlogGainedLikes)
                    {
                        Body = "TODO CENTRALIZE",
                        Title = "TODO CENTRALIZE",
                        Pars = pars
                    };

                    // First release, then push
                    scope.Complete();
                    await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform, notification).ConfigureAwait(false);
                }

                logger.LogTrace($"{nameof(NotificationVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotificationVlogLikedAsync)} - Exception in method ", e.Message);
                throw;
            }
        }

    }

}
