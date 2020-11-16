using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Handles our device registrations for azure notification hub.
    /// </summary>
    public sealed class DeviceRegistrationService : IDeviceRegistrationService
    {

        private readonly INotificationClient _notificationClient;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DeviceRegistrationService(INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
        }

        /// <summary>
        /// Registers a device and unregisters all previous registrations for
        /// the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="platform"><see cref="PushNotificationPlatform"/></param>
        /// <param name="handle">Device PNS handle</param>
        /// <returns><see cref="Task"/></returns>
        public async Task RegisterOnlyThisDeviceAsync(Guid userId, PushNotificationPlatform platform, string handle)
        {
            userId.ThrowIfNullOrEmpty();
            handle.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // First clear the existing registration if it exists
            var registrations = await _notificationRegistrationRepository.GetRegistrationsForUserAsync(userId).ConfigureAwait(false);
            if (registrations.Count() > 1) { throw new InvalidOperationException("Can't have more than one notification registration per user"); }
            if (registrations.Any())
            {
                await _notificationClient.UnregisterAsync(registrations.First()).ConfigureAwait(false);
                await _notificationRegistrationRepository.DeleteAsync(registrations.First().Id).ConfigureAwait(false);
            }

            // Create new registration and assign external id
            var registration = await _notificationClient.RegisterAsync(new NotificationRegistration
            {
                Handle = handle,
                PushNotificationPlatform = platform,
                UserId = userId
            }).ConfigureAwait(false);
            await _notificationRegistrationRepository.CreateAsync(registration).ConfigureAwait(false);

            scope.Complete();
        }

        /// <summary>
        /// Unregisters a device.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="DeviceNotRegisteredException"/> if the device
        /// was never registered.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task UnregisterAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var registrations = await _notificationRegistrationRepository.GetRegistrationsForUserAsync(userId).ConfigureAwait(false);

            // Throw if we don't have any registration
            if (!registrations.Any()) { throw new DeviceNotRegisteredException(); }

            // Throw if there is more than one registration
            if (registrations.Count() > 1) { throw new InvalidOperationException("Device still has multiple registrations, this is not allowed during Unregister operation"); }

            await _notificationClient.UnregisterAsync(registrations.First()).ConfigureAwait(false);
            await _notificationRegistrationRepository.DeleteAsync(registrations.First().Id).ConfigureAwait(false);

            scope.Complete();
        }

    }

}
