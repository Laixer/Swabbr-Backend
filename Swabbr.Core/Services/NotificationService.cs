using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
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
    /// TODO This used to contain a circular dependency for the livestream service. Implement boundary checks here!
    /// TODO Single responsibility! Inconsistent atm
    /// </summary>
    public sealed class NotificationService : INotificationService
    {

        private readonly IUserRepository _userRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly INotificationClient _notificationClient;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        private readonly INotificationBuilder _notificationBuilder;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationService(IUserRepository userRepository,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            INotificationBuilder notificationBuilder,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            _notificationBuilder = notificationBuilder ?? throw new ArgumentNullException(nameof(notificationBuilder));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(NotificationService)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Sends a <see cref="SwabbrNotification"/> to each follower of the 
        /// specified <paramref name="userId"/>.
        /// </summary>
        /// <remarks>
        /// TODO This does no checking for the livestream state.
        /// </remarks>
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
                if (pars == null) { throw new ArgumentNullException(nameof(pars)); }

                if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }

                var notification = _notificationBuilder.BuildFollowedProfileLive(pars.LiveUserId, pars.LiveLivestreamId, pars.LiveVlogId);
                var pushDetails = await _userRepository.GetFollowersPushDetailsAsync(userId).ConfigureAwait(false);
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
        /// Notifies all <see cref="SwabbrUser"/> followers that a new
        /// <see cref="Vlog"/> was posted.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotifyFollowersVlogPostedAsync(Guid userId, Guid vlogId)
        {
            try
            {
                logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Attempting notifying followers for posted vlog {vlogId} from user {userId}");
                userId.ThrowIfNullOrEmpty();
                vlogId.ThrowIfNullOrEmpty();

                // TODO State checks
                if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }
                if (!(await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false)).LivestreamId.IsNullOrEmpty()) { throw new LivestreamStateException("Vlog was still linked to livestream"); }

                var notification = _notificationBuilder.BuildFollowedProfileVlogPosted(vlogId, userId);
                var pushDetails = await _userRepository.GetFollowersPushDetailsAsync(userId).ConfigureAwait(false);
                foreach (var item in pushDetails)
                {
                    await _notificationClient.SendNotificationAsync(item.UserId, item.PushNotificationPlatform, notification).ConfigureAwait(false);
                    logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Notified user {item.UserId}");
                }
                logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Completed notifying followers for posted vlog {vlogId} from user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyFollowersVlogPostedAsync)} - Exception in method ", e.Message);
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

                var notification = _notificationBuilder.BuildRecordVlog(livestreamId, pars.VlogId, pars.RequestMoment, pars.RequestTimeout);
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

                    // First release, then push
                    scope.Complete();

                    var notification = _notificationBuilder.BuildVlogNewReaction(vlog.Id, reactionId);
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
        public async Task NotifyVlogLikedAsync(VlogLikeId vlogLikeId)
        {
            try
            {
                logger.LogTrace($"{nameof(NotifyVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");

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

                    // First release, then push
                    scope.Complete();

                    var notification = _notificationBuilder.BuildVlogGainedLike(vlogLikeId.VlogId, vlogLike.UserId);
                    await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform, notification).ConfigureAwait(false);
                }

                logger.LogTrace($"{nameof(NotifyVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyVlogLikedAsync)} - Exception in method ", e.Message);
                throw;
            }
        }

        public Task NotifyVlogRecordTimeoutAsync(Guid userId)
        {
            logger.LogError($"Implement {nameof(NotifyVlogRecordTimeoutAsync)} - called for user {userId}");
            throw new NotImplementedException(nameof(NotifyVlogRecordTimeoutAsync));
        }

    }

}
