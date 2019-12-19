using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationHubConfiguration _configuration;
        private readonly NotificationHubClient _hubClient;

        public NotificationService(IOptions<NotificationHubConfiguration> notificationHubConfigurationOptions)
        {
            _configuration = notificationHubConfigurationOptions.Value;
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(_configuration.ConnectionString, _configuration.HubName);
        }

        /// <summary>
        /// Get registration ID from Azure Notification Hub
        /// </summary>
        public async Task<string> CreateRegistrationIdAsync()
        {
            return await _hubClient.CreateRegistrationIdAsync();
        }

        /// <summary>
        /// Delete registration ID from Azure Notification Hub
        /// </summary>
        /// <param name="registrationId"></param>
        public async Task DeleteRegistrationAsync(string registrationId)
        {
            await _hubClient.DeleteRegistrationAsync(registrationId);
        }

        /// <summary>
        /// Register device to receive push notifications. 
        /// Registration ID obtained from Azure Notification Hub has to be provided
        /// Then basing on the platform (Android or iOS) a specific
        /// handle (token) obtained from Push Notification Service has to be provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceUpdate"></param>
        /// <returns></returns>
        public async Task<NotificationResponse> RegisterForPushNotificationsAsync(string id, NotificationDeviceRegistration deviceUpdate)
        {
            RegistrationDescription registrationDescription;

            switch (deviceUpdate.Platform)
            {
                case PushNotificationService.APNS:
                    registrationDescription = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case PushNotificationService.FCM:
                    registrationDescription = new FcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    return new NotificationResponse().AddErrorMessage("Please provide a correct platform notification service name.");
            }

            registrationDescription.RegistrationId = id;
            if (deviceUpdate.Tags != null)
                registrationDescription.Tags = new HashSet<string>(deviceUpdate.Tags);

            try
            {
                await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);
                return new NotificationResponse();
            }
            catch (MessagingException)
            {
                return new NotificationResponse().AddErrorMessage("Registration failed. Please register again.");
            }
        }

        /// <summary>
        /// Send push notification to specific platform (Android or iOS)
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        public async Task<NotificationResponse> SendNotificationAsync(SwabbrNotification newNotification)
        {
            try
            {
                NotificationOutcome outcome = null;

                switch (newNotification.Platform)
                {
                    case PushNotificationService.APNS:
                        if (newNotification.Tags == null)
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(newNotification.Content);
                        else
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(newNotification.Content, newNotification.Tags);
                        break;
                    case PushNotificationService.FCM:
                        if (newNotification.Tags == null)
                            outcome = await _hubClient.SendFcmNativeNotificationAsync(newNotification.Content);
                        else
                            outcome = await _hubClient.SendFcmNativeNotificationAsync(newNotification.Content, newNotification.Tags);
                        break;
                }

                if (outcome != null)
                {
                    if (!((outcome.State == NotificationOutcomeState.Abandoned) ||
                        (outcome.State == NotificationOutcomeState.Unknown)))
                        return new NotificationResponse<NotificationOutcome>();
                }

                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Notification was not sent. Please try again.");
            }

            catch (MessagingException e)
            {
                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
            }

            catch (ArgumentException e)
            {
                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
            }
        }
    }
}
