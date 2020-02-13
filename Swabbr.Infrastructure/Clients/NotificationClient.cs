using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
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

        public async Task<NotificationRegistration> RegisterAsync(DeviceRegistration deviceRegistration, IEnumerable<string> tags)
        {
            var registrationDescription = getRegistrationDescription(deviceRegistration.Platform);

            // Separate function to determine the registration description.
            RegistrationDescription getRegistrationDescription(PushNotificationPlatform platform) {
                switch (platform)
                {
                    case PushNotificationPlatform.APNS:
                        return new AppleRegistrationDescription(deviceRegistration.Handle);
                    case PushNotificationPlatform.FCM:
                        return new FcmRegistrationDescription(deviceRegistration.Handle);
                }

                throw new InvalidOperationException(nameof(platform));
            }

            // Create and use a new registration id from Azure Notification Hubs.
            registrationDescription.RegistrationId = await _hubClient.CreateRegistrationIdAsync(); ;
            registrationDescription.Tags = new HashSet<string>(tags);

            // Create a new registration for this device
            // TODO THOMAS Create a RD from an RD? What - this might seem like a good case for separation of concerns?
            var hubRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);

            return new NotificationRegistration
            {
                Id = new Guid(hubRegistration.RegistrationId), // TODO THOMAS Kan dit wel????
                Handle = hubRegistration.PnsHandle,
                Platform = deviceRegistration.Platform
            };
        }

        public async Task<NotificationResponse> SendNotification(SwabbrNotification notification, PushNotificationPlatform platform, IEnumerable<string> tags)
        {
            try
            {
                var tag = new HashSet<string>(tags);

                NotificationOutcome outcome = null;

                // TODO THOMAS Create JSON Interface 
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