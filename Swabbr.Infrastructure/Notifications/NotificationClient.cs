using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Notifications;
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
        private readonly NotificationHubClient _hubClient;
        private readonly ILogger<NotificationClient> _logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationClient(IOptions<NotificationHubConfiguration> options,
            ILogger<NotificationClient> logger)
        {
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
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
        internal async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                await _hubClient.GetRegistrationsByTagAsync("anytag", 0);
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
        internal async Task<NotificationRegistration> RegisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration is null)
            {
                throw new ArgumentNullException(nameof(internalRegistration));
            }

            // Create new registration and return the converted result
            var externalRegistration = await _hubClient.CreateRegistrationAsync(ExtractForCreation(internalRegistration));

            return new NotificationRegistration
            {
                ExternalId = externalRegistration.RegistrationId,
                Handle = internalRegistration.Handle,
                Id = internalRegistration.Id,
                PushNotificationPlatform = internalRegistration.PushNotificationPlatform,
            };
        }

        /// <summary>
        ///     Unregisters a user in Azure Notifcations Hub.
        /// </summary>
        /// <remarks>
        ///     TODO Check this
        ///     This throws an <see cref="-"/> if the 
        ///     user has no Azure Notification Hub registration.
        ///     This will log a warning if the user is registered multiple times 
        ///     in the Azure Notification Hub. This will always remove ALL existing
        ///     registrations.
        /// </remarks>
        /// <param name="internalRegistration">Internal registration object.</param>
        internal async Task UnregisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration is null)
            {
                throw new ArgumentNullException(nameof(internalRegistration));
            }

            // Throw if no registration exists
            if (!await IsRegisteredAsync(internalRegistration.Id))
            {
                _logger.LogWarning("User is not registered in Azure Notification Hub but does have an internal notification registration, doing nothing.");
            }
            else
            {
                var externalRegistrations = await _hubClient.GetRegistrationsByTagAsync(internalRegistration.Id.ToString(), 0);

                // Having multiple registrations should never happen, log a warning and skip.
                if (externalRegistrations.Count() > 1)
                {
                    _logger.LogWarning("User is registered multiple times in Azure Notification Hub, cleaning up all of them before making new registration");
                }

                // Remove all existing registrations
                foreach (var registration in externalRegistrations)
                {
                    await _hubClient.DeleteRegistrationAsync(registration.RegistrationId);
                }
            }
        }

        /// <summary>
        ///     Sends a <see cref="ScheduledNotification"/> to a specified user.
        /// </summary>
        /// <param name="userId">The user to notify.</param>
        /// <param name="platform">The user notification platform.</param>
        /// <param name="notification">The notification object.</param>
        internal async Task SendNotificationAsync(Guid userId, PushNotificationPlatform platform, SwabbrNotification notification)
        {
            switch (platform)
            {
                case PushNotificationPlatform.APNS:
                    var objApns = NotificationJsonExtractor.Extract(PushNotificationPlatform.APNS, notification);
                    var jsonApns = JsonConvert.SerializeObject(objApns);
                    await _hubClient.SendAppleNativeNotificationAsync(jsonApns, userId.ToString());
                    return;
                case PushNotificationPlatform.FCM:
                    var objFcm = NotificationJsonExtractor.Extract(PushNotificationPlatform.FCM, notification);
                    var jsonFcm = JsonConvert.SerializeObject(objFcm);
                    await _hubClient.SendFcmNativeNotificationAsync(jsonFcm, userId.ToString());
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
            if (notificationRegistration is null)
            {
                throw new ArgumentNullException(nameof(notificationRegistration));
            }

            // Use the user id as tag (as recommended by Azure Notification Hub docs)
            var tags = new List<string> { notificationRegistration.Id.ToString() };

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
            => (await _hubClient.GetRegistrationsByTagAsync(userId.ToString(), 0)).Any();
    }
}
