using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public sealed class NotificationServiceDummy : INotificationService
    {

        public Task DeleteRegistrationAsync(Guid registrationId)
        {
            return Task.CompletedTask;
        }

        public Task<NotificationResponse> RegisterUserForPushNotificationsAsync(Guid userId, DeviceRegistration deviceUpdate)
        {
            return Task.FromResult(new NotificationResponse
            {
                // Empty
            });
        }

        public Task<NotificationResponse> SendNotificationToUserAsync(SwabbrNotification notification, Guid userId)
        {
            return Task.FromResult(new NotificationResponse
            {
                // Empty
            });
        }

        public Task SendNotificationToFollowersAsync(Guid userId, Guid vlogId)
        {
            return Task.CompletedTask;

            //// Construct notification
            //var notification = new SwabbrNotification
            //{
            //    MessageContent = new SwabbrNotificationBody
            //    {
            //        //TODO: Use string constants
            //        Title = $"{nickname} is livestreaming right now!",
            //        Body = $"{nickname} has just gone live.",
            //        ClickAction = ClickActions.FOLLOWED_PROFILE_LIVE,
            //        Object = JObject.FromObject(newVlog),
            //        ObjectType = typeof(Vlog).Name
            //    }
            //};

            //// Send the notification to each follower
            //// TODO At the moment we first reach all followers, and THEN return the request. Might be risky
            //foreach (FollowRequest fr in followers)
            //{
            //    Guid followerId = fr.Id.RequesterId;
            //    await _notificationService.SendNotificationToUserAsync(notification, followerId);
            //}
        }

        public Task SendVlogTriggerToUserAsync(Guid userId, Guid liverstreamId)
        {
            return Task.CompletedTask;
        }
    }
}
