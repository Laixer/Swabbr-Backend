﻿using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Notifications;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications
{
    /// <summary>
    ///     Communicates with Azure Notification Hub to manage 
    ///     device registrations. This has no knowledge of our 
    ///     internal data store.
    /// </summary>
    /// <remarks>
    ///     The top-parameter of the <see cref="NotificationHubClient"/> 
    ///     is the index of the first result we query. The maximum 
    ///     result count is 100.
    /// </remarks>
    public class NotificationClient
    {
        private readonly NotificationHubConfiguration _options;
        private readonly NotificationHubClient _hubClient;
        private readonly ILogger<NotificationClient> _logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationClient(IOptions<NotificationHubConfiguration> options,
            ILogger<NotificationClient> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Instantiate new client only once, reuse every time.
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(options.Value.ConnectionString, options.Value.HubName);
        }

        /// <summary>
        ///     Checks if the Azure Notification Hub is available.
        /// </summary>
        /// <remarks>
        ///     This just gets registrations by some arbitrary tag.
        /// </remarks>
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                await _hubClient.GetRegistrationsByTagAsync("anytag", 0).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while checking ANH health", e.Message);
                return false;
            }
        }

        // TODO There is a state possible where a user has a registration in firebase
        // but no corresponding registration in the database. An issue is made.
        /// <summary>
        ///     Registers a user in Azure Notifications Hub.
        /// </summary>
        /// <remarks>
        ///     Check <see cref="IsRegisteredAsync(Guid)"/> first to avoid duplicate
        ///     registrations.
        /// </remarks>
        /// <param name="internalRegistration">Our internal registration object.</param>
        public async Task<NotificationRegistration> RegisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration == null)
            {
                throw new ArgumentNullException(nameof(internalRegistration));
            }
            internalRegistration.Handle.ThrowIfNullOrEmpty();

            // Create new registration and return the converted result
            var externalRegistration = await _hubClient.CreateRegistrationAsync(ExtractForCreation(internalRegistration)).ConfigureAwait(false);

            return new NotificationRegistration
            {
                ExternalId = externalRegistration.RegistrationId,
                Handle = internalRegistration.Handle,
                Id = internalRegistration.Id,
                PushNotificationPlatform = internalRegistration.PushNotificationPlatform,
                UserId = internalRegistration.UserId
            };
        }

        /// <summary>
        ///     Unregisters a user in Azure Notifcations Hub.
        /// </summary>
        /// <remarks>
        ///     This throws an <see cref="DeviceNotRegisteredException"/> if the 
        ///     user has no Azure Notification Hub registration.
        ///     This will log a warning if the user is registered multiple times 
        ///     in the Azure Notification Hub. This will always remove ALL existing
        ///     registrations.
        /// </remarks>
        /// <param name="internalRegistration">Internal registration object.</param>
        public async Task UnregisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration == null) { throw new ArgumentNullException(nameof(internalRegistration)); }
            internalRegistration.ThrowIfInvalid();

            // Throw if no registration exists
            if (!await IsRegisteredAsync(internalRegistration.UserId).ConfigureAwait(false))
            {
                _logger.LogWarning("User is not registered in Azure Notification Hub but does have an internal notification registration, cleaning all up before making new registration");
            }
            else
            {
                var externalRegistrations = await _hubClient.GetRegistrationsByTagAsync(internalRegistration.UserId.ToString(), 0).ConfigureAwait(false);

                // Having multiple registrations should never happen, log a warning and skip.
                if (externalRegistrations.Count() > 1)
                {
                    _logger.LogWarning("User is registered multiple times in Azure Notification Hub, cleaning up all of them before making new registration");
                }

                // Remove all existing registrations
                foreach (var registration in externalRegistrations)
                {
                    await _hubClient.DeleteRegistrationAsync(registration.RegistrationId).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///     Sends a <see cref="ScheduledNotification"/> to a specified user.
        /// </summary>
        /// <param name="userId">The user to notify.</param>
        /// <param name="platform">The user notification platform.</param>
        /// <param name="notification">The notification object.</param>
        public async Task SendNotificationAsync(Guid userId, PushNotificationPlatform platform, SwabbrNotification notification)
        {
            userId.ThrowIfNullOrEmpty();
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }
            // TODO Validate notification

            switch (platform)
            {
                case PushNotificationPlatform.APNS:
                    var objApns = NotificationJsonExtractor.Extract(PushNotificationPlatform.APNS, notification);
                    var jsonApns = JsonConvert.SerializeObject(objApns);
                    await _hubClient.SendAppleNativeNotificationAsync(jsonApns, userId.ToString()).ConfigureAwait(false);
                    return;
                case PushNotificationPlatform.FCM:
                    var objFcm = NotificationJsonExtractor.Extract(PushNotificationPlatform.FCM, notification);
                    var jsonFcm = JsonConvert.SerializeObject(objFcm);
                    await _hubClient.SendFcmNativeNotificationAsync(jsonFcm, userId.ToString()).ConfigureAwait(false);
                    return;
                default:
                    throw new InvalidOperationException(nameof(platform));
            }
        }

        /// <summary>
        ///     Creates a platform specific <see cref="RegistrationDescription"/>.
        /// </summary>
        /// <param name="notificationRegistration">Our internal registration.</param>
        /// <returns>Notification hub registration.</returns>
        private static RegistrationDescription ExtractForCreation(NotificationRegistration notificationRegistration)
        {
            if (notificationRegistration == null)
            {
                throw new ArgumentNullException(nameof(notificationRegistration));
            }
            notificationRegistration.Handle.ThrowIfNullOrEmpty();

            // Use the user id as tag (as recommended by Azure Notification Hub docs)
            var tags = new List<string> { notificationRegistration.UserId.ToString() };

            return notificationRegistration.PushNotificationPlatform switch
            {
                PushNotificationPlatform.APNS => new AppleRegistrationDescription(notificationRegistration.Handle, tags),
                PushNotificationPlatform.FCM => new FcmRegistrationDescription(notificationRegistration.Handle, tags),
                _ => throw new InvalidOperationException(nameof(notificationRegistration.PushNotificationPlatform)),
            };
        }

        /// <summary>
        ///     Checks if a user id has any existing registration
        ///     in Azure Notification Hub.
        /// </summary>
        /// <remarks>
        ///     This checks if the user id exists as a tag, which 
        ///     is usage as recommended by the documentation.
        /// </remarks>
        /// <param name="userId">The internal user id to check.</param>
        private async Task<bool> IsRegisteredAsync(Guid userId)
            => (await _hubClient.GetRegistrationsByTagAsync(userId.ToString(), 0).ConfigureAwait(false)).Any();
    }
}
