﻿using Laixer.Utility.Extensions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Notifications;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications
{

    /// <summary>
    /// Communicates with Azure Notification Hub to manage device registrations.
    /// This has no knowledge of our internal data store.
    /// TODO Is this smart? Maybe for double checks?
    /// TODO Should this check user existence?
    /// </summary>
    /// <remarks>
    /// The top-parameter of the <see cref="NotificationHubClient"/> is the index 
    /// of the first result we query. The maximum result count is 100.
    /// </remarks>
    public class NotificationClient : INotificationClient
    {

        private readonly NotificationHubConfiguration _hubConfiguration;
        private readonly NotificationHubClient _hubClient;
        private readonly INotificationJsonExtractor _notificationJsonExtractor;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationClient(IOptions<NotificationHubConfiguration> options,
            ILoggerFactory loggerFactory,
            INotificationJsonExtractor notificationJsonExtractor)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            _hubConfiguration = options.Value;
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(_hubConfiguration.ConnectionString, _hubConfiguration.HubName);
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(NotificationClient)) : throw new ArgumentNullException(nameof(loggerFactory));
            _notificationJsonExtractor = notificationJsonExtractor ?? throw new ArgumentNullException(nameof(notificationJsonExtractor));
        }

        /// <summary>
        /// Registers a <see cref="SwabbrUser"/> in Azure Notifications Hub.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="InvalidOperationException"/> if the user
        /// is already registered in Azure Notification Hub.
        /// TODO Do we want this?
        /// </remarks>
        /// <param name="inernalRegistration"><see cref="NotificationRegistration"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task<NotificationRegistration> RegisterAsync(NotificationRegistration inernalRegistration)
        {
            if (inernalRegistration == null) { throw new ArgumentNullException(nameof(inernalRegistration)); }
            // notificationRegistration.Id.ThrowIfNullOrEmpty(); // TODO Do we want this? Specify
            // notificationRegistration.ExternalId.ThrowIfNullOrEmpty(); TODO Do we want this? Specify
            inernalRegistration.Handle.ThrowIfNullOrEmpty();

            // TODO There is a state possible where a user has a registration in firebase
            // but no corresponding registration in the database. An issue is made.

            // Throw if a registration already exists
            // TODO This is ugly syntax
            // TODO This does a lazy evaluation!
            if (!string.IsNullOrEmpty(inernalRegistration.ExternalId) &&
                new List<RegistrationDescription>(
                    await _hubClient.GetRegistrationsByTagAsync(inernalRegistration.UserId.ToString(), 0)
                    .ConfigureAwait(false))
                    .Any()) { throw new InvalidOperationException("User is already registered on Azure Notification Hub"); }

            // Create new registration and return the converted result
            var externalRegistration = await _hubClient.CreateRegistrationAsync(ExtractForCreation(inernalRegistration)).ConfigureAwait(false);
            return new NotificationRegistration
            {
                ExternalId = externalRegistration.RegistrationId,
                Handle = inernalRegistration.Handle,
                Id = inernalRegistration.Id,
                PushNotificationPlatform = inernalRegistration.PushNotificationPlatform,
                UserId = inernalRegistration.UserId
            };
        }

        /// <summary>
        /// Unregisters a <see cref="SwabbrUser"/> in Azure Notifcations Hub.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="DeviceNotRegisteredException"/> if the 
        /// user has no Azure Notification Hub registration.
        /// This will log a warning if the user is registered multiple times 
        /// in the Azure Notification Hub. This will always remove ALL existing
        /// registrations.
        /// </remarks>
        /// <param name="internalRegistration"><see cref="NotificationRegistration"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task UnregisterAsync(NotificationRegistration internalRegistration)
        {
            if (internalRegistration == null) { throw new ArgumentNullException(nameof(internalRegistration)); }
            internalRegistration.ThrowIfInvalid();

            // Throw if no registration exists
            var externalRegistrations = new List<RegistrationDescription>(
                await _hubClient.GetRegistrationsByTagAsync(internalRegistration.UserId.ToString(), 0)
                .ConfigureAwait(false));
            if (!externalRegistrations.Any()) { throw new DeviceNotRegisteredException("User is not registered in Azure Notification Hub"); }
            if (externalRegistrations.Count > 1) { logger.LogWarning("User is registered multiple times in Azure Notification Hub, cleaning up all of them before making new registration"); }

            // Remove the registrations
            foreach (var registration in externalRegistrations)
            {
                await _hubClient.DeleteRegistrationAsync(registration.RegistrationId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a <see cref="ScheduledNotification"/> to a specified user.
        /// </summary>
        /// <param name="internalRegistration"><see cref="NotificationRegistration"/></param>
        /// <param name="notification"><see cref="SwabbrNotification"/></param>
        /// <returns><see cref=""/></returns>
        public async Task SendNotificationAsync(Guid userId, PushNotificationPlatform platform, SwabbrNotification notification)
        {
            userId.ThrowIfNullOrEmpty();

            if (notification == null) { throw new ArgumentNullException(nameof(notification)); }
            notification.Title.ThrowIfNullOrEmpty();
            notification.Body.ThrowIfNullOrEmpty();

            switch (platform)
            {
                case PushNotificationPlatform.APNS:
                    var objApns = _notificationJsonExtractor.Extract(PushNotificationPlatform.APNS, notification);
                    var jsonApns = JsonConvert.SerializeObject(objApns);
                    var resultApns = await _hubClient.SendAppleNativeNotificationAsync(jsonApns, userId.ToString()).ConfigureAwait(false);
                    return;
                case PushNotificationPlatform.FCM:
                    var objFcm = _notificationJsonExtractor.Extract(PushNotificationPlatform.FCM, notification);
                    var jsonFcm = JsonConvert.SerializeObject(objFcm);
                    var resultFcm = await _hubClient.SendFcmNativeNotificationAsync(jsonFcm, userId.ToString()).ConfigureAwait(false);
                    return;
            }

            throw new InvalidOperationException(nameof(platform));
        }

        /// <summary>
        /// Creates a platform specific <see cref="RegistrationDescription"/>.
        /// TODO Do we need more information here?
        /// </summary>
        /// <param name="platform"><see cref="PushNotificationPlatform"/></param>
        /// <param name="externalRegistrationId">External registration id</param>
        /// <returns><see cref="RegistrationDescription"/></returns>
        private RegistrationDescription ExtractForCreation(NotificationRegistration notificationRegistration)
        {
            if (notificationRegistration == null) { throw new ArgumentNullException(nameof(notificationRegistration)); }
            notificationRegistration.Handle.ThrowIfNullOrEmpty();

            // Use the user id as tag (as recommended by Azure Notification Hub docs)
            var tags = new List<string> { notificationRegistration.UserId.ToString() };

            switch (notificationRegistration.PushNotificationPlatform)
            {
                case PushNotificationPlatform.APNS:
                    return new AppleRegistrationDescription(notificationRegistration.Handle, tags);
                case PushNotificationPlatform.FCM:
                    return new FcmRegistrationDescription(notificationRegistration.Handle, tags);
            }

            throw new InvalidOperationException(nameof(notificationRegistration.PushNotificationPlatform));
        }

        /// <summary>
        /// Gets a <see cref="NotificationRegistration"/> from Azure Notification
        /// Hub.
        /// TODO Is this used? Else DELETE
        /// </summary>
        /// <param name="notificationRegistration"><see cref="NotificationRegistration"/></param>
        /// <returns><see cref="RegistrationDescription"/></returns>
        private async Task<RegistrationDescription> GetAsync(NotificationRegistration notificationRegistration)
        {
            if (notificationRegistration == null) { throw new ArgumentNullException(nameof(notificationRegistration)); }
            notificationRegistration.ThrowIfInvalid();

            switch (notificationRegistration.PushNotificationPlatform)
            {
                case PushNotificationPlatform.APNS:
                    return await _hubClient.GetRegistrationAsync<AppleRegistrationDescription>(notificationRegistration.ExternalId).ConfigureAwait(false);
                case PushNotificationPlatform.FCM:
                    return await _hubClient.GetRegistrationAsync<FcmRegistrationDescription>(notificationRegistration.ExternalId).ConfigureAwait(false);
            }

            throw new InvalidOperationException(nameof(notificationRegistration.PushNotificationPlatform));
        }











        //public async Task<RegistrationDescription> GetRegistrationAsync(string registrationId)
        //{
        //    return await _hubClient.GetRegistrationAsync<RegistrationDescription>(registrationId);
        //}

        //public async Task DeleteRegistrationAsync(string registrationId)
        //{
        //    await _hubClient.DeleteRegistrationAsync(registrationId);
        //}

        //public async Task<NotificationRegistration> RegisterAsync(DeviceRegistration deviceRegistration, IEnumerable<string> tags)
        //{
        //    var registrationDescription = getRegistrationDescription(deviceRegistration.Platform);

        //    // Separate function to determine the registration description.
        //    RegistrationDescription getRegistrationDescription(PushNotificationPlatform platform)
        //    {
        //        switch (platform)
        //        {
        //            case PushNotificationPlatform.APNS:
        //                return new AppleRegistrationDescription(deviceRegistration.Handle);
        //            case PushNotificationPlatform.FCM:
        //                return new FcmRegistrationDescription(deviceRegistration.Handle);
        //        }

        //        throw new InvalidOperationException(nameof(platform));
        //    }

        //    // Create and use a new registration id from Azure Notification Hubs.
        //    registrationDescription.RegistrationId = await _hubClient.CreateRegistrationIdAsync(); ;
        //    registrationDescription.Tags = new HashSet<string>(tags);

        //    // Create a new registration for this device
        //    // TODO THOMAS Create a RD from an RD? This might seem like a good case for separation of concerns?
        //    var hubRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(registrationDescription);

        //    return new NotificationRegistration
        //    {
        //        Id = new Guid(hubRegistration.RegistrationId), // TODO THOMAS Kan dit wel????
        //        Handle = hubRegistration.PnsHandle,
        //        Platform = deviceRegistration.Platform
        //    };
        //}

        //public async Task<NotificationResponse> SendNotification(SwabbrNotification notification, PushNotificationPlatform platform, IEnumerable<string> tags)
        //{

        //    try
        //    {
        //        var tag = new HashSet<string>(tags);

        //        NotificationOutcome outcome = null;

        //        // TODO THOMAS Create JSON Interface 
        //        switch (platform)
        //        {
        //            case PushNotificationPlatform.APNS:
        //                {
        //                    // Get JSON content string for Apple PNS
        //                    string jsonContent = notification.MessageContent.GetAppleContentJSON().ToString();
        //                    outcome = await _hubClient.SendAppleNativeNotificationAsync(jsonContent, tag);
        //                    break;
        //                }
        //            case PushNotificationPlatform.FCM:
        //                {
        //                    // Get JSON content string for FCM
        //                    string jsonContent = notification.MessageContent.GetFcmContentJSON().ToString();
        //                    outcome = await _hubClient.SendFcmNativeNotificationAsync(jsonContent, tag);
        //                    break;
        //                }
        //        }

        //        // TODO THOMAS Might just want to make the previous bit throw on error?
        //        if (outcome != null)
        //        {
        //            if (!(outcome.State == NotificationOutcomeState.Abandoned ||
        //                  outcome.State == NotificationOutcomeState.Unknown))
        //            {
        //                return new NotificationResponse<NotificationOutcome>();
        //            }
        //        }

        //        return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Notification was not sent. Please try again.");
        //    }
        //    catch (MessagingException e)
        //    {
        //        return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage(e.Message);
        //    }
        //    catch (EntityNotFoundException)
        //    {
        //        return new NotificationResponse<NotificationOutcome>().SetAsFailureResponse().AddErrorMessage("Could not find any stored Notification Hub Registrations.");
        //    }
        //}

    }
}