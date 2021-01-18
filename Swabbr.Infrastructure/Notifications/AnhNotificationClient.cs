using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Notifications;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class AnhNotificationClient : INotificationClient
    {
        private readonly NotificationHubClient _hubClient;
        private readonly ILogger<AnhNotificationClient> _logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AnhNotificationClient(IOptions<AzureNotificationHubConfiguration> options,
            ILogger<AnhNotificationClient> logger,
            IConfiguration configuration)
        {
            if (options is null || options.Value is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Instantiate new client only once, reuse every time.
            var connectionString = configuration.GetConnectionString(options.Value.ConnectionStringName);
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString, options.Value.HubName);
        }

        /// <summary>
        ///     Checks if the Azure Notification Hub is available.
        /// </summary>
        /// <remarks>
        ///     This simply gets registrations by some arbitrary tag.
        /// </remarks>
        public async Task TestServiceAsync()
            => await _hubClient.GetRegistrationsByTagAsync("anytag", 0);

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
        ///     This will log a warning if the user is registered multiple times 
        ///     in the Azure Notification Hub. This will always remove ALL existing
        ///     registrations.
        /// </remarks>
        /// <param name="internalRegistration">Internal registration object.</param>
        public async Task UnregisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration is null)
            {
                throw new ArgumentNullException(nameof(internalRegistration));
            }

            // Log and exit if no registration exists
            if (!await IsRegisteredAsync(internalRegistration.Id))
            {
                _logger.LogWarning("User is not registered in Azure Notification Hub but does have an internal notification registration, doing nothing.");
                return;
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
                await Task.WhenAll(externalRegistrations.Select(x => _hubClient.DeleteRegistrationAsync(x.RegistrationId)));
            }
        }

        /// <summary>
        ///     Sends a <see cref="ScheduledNotification"/> to a specified user.
        /// </summary>
        /// <param name="pushDetails">Details on how to reach the user.</param>
        /// <param name="notification">The notification object.</param>
        public async Task SendNotificationAsync(UserPushNotificationDetails pushDetails, SwabbrNotification notification)
        {
            if (pushDetails is null)
            {
                throw new ArgumentNullException(nameof(pushDetails));
            }

            switch (pushDetails.PushNotificationPlatform)
            {
                case PushNotificationPlatform.APNS:
                    var objApns = NotificationJsonExtractor.Extract(PushNotificationPlatform.APNS, notification);
                    var jsonApns = JsonSerializer.Serialize(objApns);
                    await _hubClient.SendAppleNativeNotificationAsync(jsonApns, pushDetails.UserId.ToString());
                    return;
                case PushNotificationPlatform.FCM:
                    var objFcm = NotificationJsonExtractor.Extract(PushNotificationPlatform.FCM, notification);
                    var jsonFcm = JsonSerializer.Serialize(objFcm);
                    await _hubClient.SendFcmNativeNotificationAsync(jsonFcm, pushDetails.UserId.ToString());
                    return;
                default:
                    throw new InvalidOperationException(nameof(pushDetails.PushNotificationPlatform));
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
