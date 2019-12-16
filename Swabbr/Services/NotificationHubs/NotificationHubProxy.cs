using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Swabbr.Api.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Api.Services.NotificationHubs
{
    public class NotificationHubProxy
    {
        private readonly NotificationHubConfiguration _configuration;
        private readonly NotificationHubClient _hubClient;

        public NotificationHubProxy(NotificationHubConfiguration configuration)
        {
            _configuration = configuration;
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(_configuration.ConnectionString, _configuration.HubName);
        }

        /// 
        /// <summary>
        /// Get registration ID from Azure Notification Hub
        /// </summary>
        public async Task<string> CreateRegistrationId()
        {
            return await _hubClient.CreateRegistrationIdAsync();
        }

        /// 
        /// <summary>
        /// Delete registration ID from Azure Notification Hub
        /// </summary>
        /// <param name="registrationId"></param>
        public async Task DeleteRegistration(string registrationId)
        {
            await _hubClient.DeleteRegistrationAsync(registrationId);
        }

        /// <summary>
        /// Register device to receive push notifications. 
        /// Registration ID ontained from Azure Notification Hub has to be provided
        /// Then basing on platform (Android, iOS or Windows) specific
        /// handle (token) obtained from Push Notification Service has to be provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceUpdate"></param>
        /// <returns></returns>
        public async Task<HubResponse> RegisterForPushNotifications(string id, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registrationDescription = null;

            switch (deviceUpdate.Platform)
            {
                case MobilePlatform.APNS:
                    registrationDescription = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case MobilePlatform.FCM:
                    registrationDescription = new FcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    return new HubResponse().AddErrorMessage("Please provide a correct platform notification service name.");
            }

            registrationDescription.RegistrationId = id;
            if (deviceUpdate.Tags != null)
                registrationDescription.Tags = new HashSet<string>(deviceUpdate.Tags);

            try
            {
                await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);
                return new HubResponse();
            }
            catch (MessagingException)
            {
                return new HubResponse().AddErrorMessage("Registration failed. Please register again.");
            }
        }

        /// <summary>
        /// Send push notification to specific platform (Android, iOS or Windows)
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        public async Task<HubResponse<NotificationOutcome>> SendNotification(Notification newNotification)
        {
            try
            {
                NotificationOutcome outcome = null;

                switch (newNotification.Platform)
                {
                    case MobilePlatform.APNS:
                        if (newNotification.Tags == null)
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(newNotification.Content);
                        else
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(newNotification.Content, newNotification.Tags);
                        break;
                    case MobilePlatform.FCM:
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
                        return new HubResponse<NotificationOutcome>();
                }

                return new HubResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Notification was not sent. Please send it again.");
            }

            catch (MessagingException ex)
            {
                return new HubResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(ex.Message);
            }

            catch (ArgumentException ex)
            {
                return new HubResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(ex.Message);
            }
        }
    }
}
