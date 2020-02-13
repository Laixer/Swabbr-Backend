using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Swabbr.Core.Interfaces;

namespace Swabbr.Core.Services
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        private readonly INotificationClient _notificationClient;

        public NotificationService(
            INotificationRegistrationRepository notificationRegistrationRepository,
            INotificationClient notificationClient
            )
        {
            _notificationRegistrationRepository = notificationRegistrationRepository;
            _notificationClient = notificationClient;
        }

        public Task DeleteRegistrationAsync(Guid registrationId)
        {
            throw new NotImplementedException();
        }

        // TODO THOMAS This should be transactional. 
        // TODO THOMAS The way user tags are used is very bad smelling
        // TODO THOMAS Why return a new notification response? 
        // TODO THOMAs The notification client should probably also commit to the database. It's a full process, and should be treated as a transaction.
        public async Task<NotificationResponse> RegisterUserForPushNotificationsAsync(Guid userId, DeviceRegistration deviceRegistration)
        {
            List<string> userTag = new List<string>()
            {
                userId.ToString()
            };

            // Create a new registration
            var registration = await _notificationClient.RegisterAsync(deviceRegistration, userTag);

            // Bind the registration to the requested user
            registration.UserId = userId;

            // Store the newly created registration in the database
            await _notificationRegistrationRepository.CreateAsync(registration);

            return new NotificationResponse();
        }

        public async Task<NotificationResponse> SendNotificationToUserAsync(SwabbrNotification notification, Guid userId)
        {
            List<string> userTag = new List<string>()
            {
                userId.ToString()
            };

            // Obtain the stored notification registration for this user. 
            // This is needed to determine the platform for which the notification should be sent out.
            var registration = await _notificationRegistrationRepository.GetByUserIdAsync(userId);

            return await _notificationClient.SendNotification(notification, registration.Platform, userTag);
        }
    }
}
