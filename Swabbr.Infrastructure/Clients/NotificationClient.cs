using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class NotificationClient : INotificationClient
    {
        private readonly NotificationHubConfiguration _hubConfiguration;
        private readonly NotificationHubClient _hubClient;

        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        public NotificationClient(
            IOptions<NotificationHubConfiguration> notificationHubConfigurationOptions,
            INotificationRegistrationRepository notificationRegistrationRepository
            )
        {
            _hubConfiguration = notificationHubConfigurationOptions.Value;
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(_hubConfiguration.ConnectionString, _hubConfiguration.HubName);
            _notificationRegistrationRepository = notificationRegistrationRepository;
        }

        /// <summary>
        /// Get registration from Azure Notification Hub
        /// </summary>
        public async Task<RegistrationDescription> GetRegistrationAsync(string registrationId)
        {
            return await _hubClient.GetRegistrationAsync<RegistrationDescription>(registrationId);
        }

        /// <summary>
        /// Delete registration ID from Azure Notification Hub
        /// </summary>
        /// <param name="registrationId"></param>
        public async Task DeleteRegistrationAsync(string registrationId)
        {
            await _hubClient.DeleteRegistrationAsync(registrationId);
        }

        public async Task<NotificationRegistration> RegisterAsync(DeviceRegistration deviceRegistration, IList<string> tags)
        {
            RegistrationDescription registrationDescription;

            // TODO THOMAS Make this a separate function to prevent the undefined state
            // TODO THOMAS Also, this can probably be generic? Inheritance for the win?
            switch (deviceRegistration.Platform)
            {
                case PushNotificationPlatform.APNS:
                    {
                        registrationDescription = new AppleRegistrationDescription(deviceRegistration.Handle);
                        break;
                    }
                case PushNotificationPlatform.FCM:
                    {
                        registrationDescription = new FcmRegistrationDescription(deviceRegistration.Handle);
                        break;
                    }
                default:
                    {
                        throw new Exception("No platform notification service was provided.");
                    }
            }

            // Create and use a new registration id from Azure Notification Hubs.
            registrationDescription.RegistrationId = await _hubClient.CreateRegistrationIdAsync(); ;
            registrationDescription.Tags = new HashSet<string>(tags);

            try
            {
                // Create a new registration for this device
                // TODO THOMAS Create a RD from an RD? What - this might seem like a good case for separation of concerns?
                RegistrationDescription hubRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);

                return new NotificationRegistration
                {
                    RegistrationId = hubRegistration.RegistrationId,
                    Handle = hubRegistration.PnsHandle,
                    Platform = deviceRegistration.Platform
                };
            }
            catch (MessagingException)
            {
                // TODO THOMAS Hmmmmmmm
                throw;
            }
        }

        public async Task<NotificationResponse> SendNotification(SwabbrNotification notification, PushNotificationPlatform platform, IList<string> tags)
        {
            try
            {
                var tag = new HashSet<string>(tags);

                NotificationOutcome outcome = null;

                // TODO THOMAS json content string is dangerous
                switch (platform)
                {
                    case PushNotificationPlatform.APNS:
                        {
                            // Get JSON content string for Apple PNS
                            string jsonContent = notification.MessageContent.GetAppleContentJSON().ToString();
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(jsonContent, tag);
                            break;
                        }
                    case PushNotificationPlatform.FCM:
                        {
                            // Get JSON content string for FCM
                            string jsonContent = notification.MessageContent.GetFcmContentJSON().ToString();
                            outcome = await _hubClient.SendFcmNativeNotificationAsync(jsonContent, tag);
                            break;
                        }
                }

                // TODO THOMAS Might just want to make the previous bit throw on error?
                if (outcome != null)
                {
                    if (!(outcome.State == NotificationOutcomeState.Abandoned ||
                          outcome.State == NotificationOutcomeState.Unknown))
                    {
                        return new NotificationResponse<NotificationOutcome>();
                    }
                }

                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Notification was not sent. Please try again.");
            }
            // TODO THOMAS This doesn't seem right, revisit 
            catch (MessagingException e)
            {
                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
            }
            catch (ArgumentException e)
            {
                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
            }
            catch (EntityNotFoundException)
            {
                return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Could not find any stored Notification Hub Registrations.");
            }
        }
    }
}