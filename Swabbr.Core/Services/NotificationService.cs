using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Contains functionality to handle notification operations.
    /// </summary>
    public sealed class NotificationService : INotificationService
    {

        private readonly IUserRepository _userRepository;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamingService _livestreamingService;
        private readonly INotificationClient _notificationClient;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationService(IUserRepository userRepository,
            ILivestreamRepository livestreamRepository,
            ILivestreamingService livestreamingService,
            INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
        }

        /// <summary>
        /// Sends a <see cref="SwabbrNotification"/> to each follower of the 
        /// specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task NotifyFollowersProfileLiveAsync(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }
            if (!await _livestreamingService.IsLivestreamValidForFollowersAsync(livestreamId, userId).ConfigureAwait(false))
            {
                throw new InvalidOperationException("Livestream is not ready for followers");
            }

            var notification = new SwabbrNotification(NotificationAction.FollowedProfileLive)
            {
                Body = "TODO CENTRALIZE Your following user profile is live!",
                Title = "TODO CENTRALIZE User is live!"
            };

            var pushDetails = await _userRepository.GetFollowersPushDetailsAsync(userId).ConfigureAwait(false);

            // TODO Future optimization
            // var detailsAndroid = new List<UserPushNotificationDetails>();
            // var detailsIos = new List<UserPushNotificationDetails>();

            foreach (var item in pushDetails)
            {
                await _notificationClient.SendNotificationAsync(item.UserId, item.PushNotificationPlatform, notification).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// DEBUG FUNCTION
        /// </summary>
        public async Task TestNotifationAsync(Guid userId, string message)
        {
            userId.ThrowIfNullOrEmpty();
            message.ThrowIfNullOrEmpty();
            var registrations = await _notificationRegistrationRepository.GetRegistrationsForUserAsync(userId).ConfigureAwait(false);
            if (!registrations.Any()) { throw new DeviceNotRegisteredException(); }
            await _notificationClient.SendNotificationAsync(userId, PushNotificationPlatform.FCM,
                new SwabbrNotification(NotificationAction.VlogRecordRequest)
                {
                    Body = "This is my body",
                    CreatedAt = DateTimeOffset.Now,
                    Title = message,
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a notification to a <see cref="SwabbrUser"/> when we have decided
        /// that the user should start streaming.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/>id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task VlogRecordRequestAsync(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }
            if (!await _livestreamingService.IsLivestreamValidForFollowersAsync(livestreamId, userId).ConfigureAwait(false))
            {
                //throw new InvalidOperationException("Livestream is not ready for followers");
            }

            var notification = new SwabbrNotification(NotificationAction.VlogRecordRequest)
            {
                Body = "TODO CENTRALIZE A livestream is ready to stream whatever you want!",
                Title = "TODO CENTRALIZE Time to record a vlog!"
            };

            var pushDetails = await _userRepository.GetPushDetailsAsync(userId).ConfigureAwait(false);
            await _notificationClient.SendNotificationAsync(pushDetails.UserId, pushDetails.PushNotificationPlatform, notification).ConfigureAwait(false);
        }
    }

}
