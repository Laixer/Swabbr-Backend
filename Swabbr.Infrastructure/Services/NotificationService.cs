﻿using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
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
        private readonly NotificationHubConfiguration _hubConfiguration;
        private readonly NotificationHubClient _hubClient;

        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        public NotificationService(
            IOptions<NotificationHubConfiguration> notificationHubConfigurationOptions,
            INotificationRegistrationRepository notificationRegistrationRepository
            )
        {
            _hubConfiguration = notificationHubConfigurationOptions.Value;
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(_hubConfiguration.ConnectionString, _hubConfiguration.HubName);
            _notificationRegistrationRepository = notificationRegistrationRepository;
        }

        /// <summary>
        /// Create and return new registration ID from Azure Notification Hub
        /// </summary>
        public async Task<string> CreateRegistrationIdAsync()
        {
            return await _hubClient.CreateRegistrationIdAsync();
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

        /// <summary>
        /// Register device to receive push notifications. Registration ID obtained from Azure
        /// Notification Hub has to be provided Then basing on the platform (Android or iOS) a
        /// specific handle (token) obtained from Push Notification Service has to be provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceRegistration"></param>
        /// <returns></returns>
        public async Task<NotificationResponse> RegisterUserForPushNotificationsAsync(string id, Guid userId, DeviceRegistration deviceRegistration)
        {
            RegistrationDescription registrationDescription;

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
                        return new NotificationResponse().AddErrorMessage("Please provide a correct platform notification service.");
                    }
            }

            registrationDescription.RegistrationId = id;

            List<string> tags = new List<string>
            {
                userId.ToString()
            };

            registrationDescription.Tags = new HashSet<string>(tags);

            try
            {
                // Create a new registration for this device
                RegistrationDescription hubRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);

                // Store registration in storage
                await _notificationRegistrationRepository.CreateAsync(new NotificationRegistration
                {
                    RegistrationId = hubRegistration.RegistrationId,
                    Handle = hubRegistration.PnsHandle,
                    Platform = deviceRegistration.Platform,
                    UserId = userId
                });

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
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task<NotificationResponse> SendNotificationToUserAsync(SwabbrNotification notification, Guid userId)
        {
            List<string> userTag = new List<string>()
            {
                userId.ToString()
            };

            try
            {
                NotificationOutcome outcome = null;

                //TODO What to do if there is no registration? Can't send a notification
                //TODO Overwrite registration when >=1 registration
                //!IMPORTANT
                // Obtain the notification registration for this user
                var registration = await _notificationRegistrationRepository.GetByUserIdAsync(userId);

                switch (registration.Platform)
                {
                    case PushNotificationPlatform.APNS:
                        {
                            // Get JSON content string for Apple PNS
                            string jsonContent = notification.MessageContent.GetAppleContentJSON().ToString();
                            outcome = await _hubClient.SendAppleNativeNotificationAsync(jsonContent, userTag);
                            break;
                        }
                    case PushNotificationPlatform.FCM:
                        {
                            // Get JSON content string for FCM
                            string jsonContent = notification.MessageContent.GetFcmContentJSON().ToString();
                            outcome = await _hubClient.SendFcmNativeNotificationAsync(jsonContent, userTag);
                            break;
                        }
                }

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
        }
    }
}