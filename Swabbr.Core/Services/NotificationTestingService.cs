using Laixer.Utility.Extensions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Used to test notifications.
    /// </summary>
    public sealed class NotificationTestingService : INotificationTestingService
    {

        private readonly IUserService _userService;
        private readonly INotificationClient _notificationClient;
        private readonly INotificationBuilder _notificationBuilder;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationTestingService(IUserService userService,
            INotificationClient notificationClient,
            INotificationBuilder notificationBuilder)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationBuilder = notificationBuilder ?? throw new ArgumentNullException(nameof(notificationBuilder));
        }

        public async Task NotifyFollowersProfileLiveAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            var userPushDetails = await _userService.GetUserPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform,
                _notificationBuilder.BuildFollowedProfileLive(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())).ConfigureAwait(false);
        }

        public async Task NotifyFollowersVlogPostedAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            var userPushDetails = await _userService.GetUserPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform,
                _notificationBuilder.BuildFollowedProfileVlogPosted(Guid.NewGuid(), Guid.NewGuid())).ConfigureAwait(false);
        }

        public async Task NotifyReactionPlacedAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            var userPushDetails = await _userService.GetUserPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform,
                _notificationBuilder.BuildVlogNewReaction(Guid.NewGuid(), Guid.NewGuid())).ConfigureAwait(false);
        }

        public async Task NotifyVlogLikedAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            var userPushDetails = await _userService.GetUserPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform,
                _notificationBuilder.BuildVlogGainedLike(Guid.NewGuid(), Guid.NewGuid())).ConfigureAwait(false);
        }

        public async Task NotifyVlogRecordRequestAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            var userPushDetails = await _userService.GetUserPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform,
                _notificationBuilder.BuildRecordVlog(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, TimeSpan.FromMinutes(5))).ConfigureAwait(false);
        }

        public Task NotifyVlogRecordTimeoutAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

    }

}
