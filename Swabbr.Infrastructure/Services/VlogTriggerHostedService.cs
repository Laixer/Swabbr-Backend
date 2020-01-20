using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class VlogTriggerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IVlogRepository _vlogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamingService _livestreamingService;
        private readonly INotificationService _notificationService;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        public VlogTriggerHostedService(
            IVlogRepository vlogRepository, 
            IUserRepository userRepository,
            ILivestreamRepository livestreamRepository,
            ILivestreamingService livestreamingService)
        {
            _vlogRepository = vlogRepository;
            _userRepository = userRepository;
            _livestreamRepository = livestreamRepository;
            _livestreamingService = livestreamingService;
        }

        /// <summary>
        /// An event that determines if a vlog request should be triggered and sent out to the user.
        /// </summary>
        /// <param name="state"></param>
        public async void OnTriggerEventAsync(object state)
        {
            //TODO Determine which users to send a vlog request

            //TODO  For each user that should be triggered:

            //TODO  Reserve a livestream for this user
            Guid exampleUserGuid = new Guid("d206ad6c-0bdc-4f17-903a-3b5c260de8c2");
            var connectionDetails = await _livestreamingService.ReserveLiveStreamForUserAsync(exampleUserGuid);

            // Get the notification hub registration details for this user
            var registration = await _notificationRegistrationRepository.GetByUserIdAsync(exampleUserGuid);

            // Send notification to this user with the livestream connection details
            var notification = new SwabbrNotification
            {
                Platform = registration.Platform,
                MessageContent = new SwabbrMessage
                {
                    Title = "Time to record a vlog!",
                    Body = "It's time to record a vlog!",
                    Data = connectionDetails,
                    DataType = nameof(connectionDetails),
                    TimeStamp = DateTime.Now
                }
            };

            await _notificationService.SendNotificationToUserAsync(notification, exampleUserGuid);

            // Set time-out
            _ = WaitForTimeoutAsync(exampleUserGuid, connectionDetails.Id);
        }

        public async Task WaitForTimeoutAsync(Guid userId, string livestreamId)
        {
            // Wait 10 minutes
            await Task.Delay(TimeSpan.FromMinutes(10));

            //TODO For later:
            /*
                             * If a user is currently streaming at this point (livestream has started),
                             * extend the delay before deactivating the stream?
                             */

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            // Check if the stream is active (reserved by the user)
            if (livestream.IsActive)
            {
                // If it is, we ensure the livestream is stopped
                // and reset the availability of the stream.
                await _livestreamingService.StopStreamAsync(livestreamId);

                livestream.IsActive = false;
                livestream.UserId = Guid.Empty;
                await _livestreamRepository.UpdateAsync(livestream);
            }
        }

        private async Task<bool> ShouldUserRecordVlog(Guid userId)
        {
            // TODO Determine whether the user is elligible for reserving a livestream (recording a vlog)

            User user = await (_userRepository.GetByIdAsync(userId));

            // TODO Currently determining this arbitrarily for testing purposes.
            return user.Email.Equals("user@example.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(OnTriggerEventAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, TimeSpan.Zero.Ticks);
            await Task.CompletedTask;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}