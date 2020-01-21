using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            ILivestreamingService livestreamingService,
            INotificationService notificationService,
            INotificationRegistrationRepository notificationRegistrationRepository
            )
        {
            _vlogRepository = vlogRepository;
            _userRepository = userRepository;
            _livestreamRepository = livestreamRepository;
            _livestreamingService = livestreamingService;
            _notificationService = notificationService;
            _notificationRegistrationRepository = notificationRegistrationRepository;
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

            try
            {
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
                        Data = JObject.FromObject(connectionDetails),
                        DataType = nameof(connectionDetails),
                        TimeStamp = DateTime.Now
                    }
                };

                await _livestreamingService.StartStreamAsync(connectionDetails.Id);

                System.Diagnostics.Debug.WriteLine($"Started livestream for user {exampleUserGuid} livestream id: {connectionDetails.Id}. Sending notification in 1 minute.");

                // Wait 2 minutes before sending the notification to ensure the stream has started.
                await Task.Delay(TimeSpan.FromMinutes(2));

                // Send the notification containing the stream connection details to the user.
                await _notificationService.SendNotificationToUserAsync(notification, exampleUserGuid);

                // Set time-out
                await WaitForTimeoutAsync(exampleUserGuid, connectionDetails.Id);
            }
            catch (Exception e)
            {
                //TODO Handle exception

                // If we get here, either no stream could be reserved,
                // something went wrong with receiving hub registrations (in which case we can not send any notifications)
                // or something went wrong while sending out a notification (in which case we should possibly try to send it again)
            }
        }

        public async Task WaitForTimeoutAsync(Guid userId, string livestreamId)
        {
            // Wait 10 minutes
            await Task.Delay(TimeSpan.FromMinutes(5));

            System.Diagnostics.Debug.WriteLine($"Livestream {livestreamId} has timed out.");

            //TODO For later:
            /*
                             * If a user is currently streaming at this point (livestream has started),
                             * extend the delay before deactivating the stream?
                             */

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            // Check if the stream is active (reserved by the user)
            if (livestream.IsActive)
            {
                try
                {
                    // If it is, we ensure the livestream is stopped
                    // and reset the availability of the stream.
                    await _livestreamingService.StopStreamAsync(livestreamId);

                    livestream.IsActive = false;
                    livestream.UserId = Guid.Empty;
                    await _livestreamRepository.UpdateAsync(livestream);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception thrown in timeout. Message: {e.Message}");
                }
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
            //TODO Timer currently disabled to prevent spamming with notifications
            //_timer = new Timer(OnTriggerEventAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
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