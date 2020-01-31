using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class VlogTriggerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        /// <summary>
        /// The amount of time before disposing a livestream
        /// </summary>
        public static readonly TimeSpan TIMESPAN_TIMEOUT = TimeSpan.FromMinutes(30);

        /// <summary>
        /// (TEMPORARY) The amount of time between executing vlog triggers (sending out notifications).
        /// </summary>
        public static readonly TimeSpan TIMESPAN_INTERVAL = TimeSpan.FromMinutes(30);

        /// <summary>
        /// The amount of time that is required to wait while starting up a livestream.
        /// </summary>
        public static readonly TimeSpan TIMESPAN_START_STREAM = TimeSpan.FromMinutes(2);

        /// <summary>
        /// The maximum amount of livestreams to keep in storage
        /// </summary>
        private const int MAX_STREAM_POOL_SIZE = 100;

        /// <summary>
        /// Livestream create request limit
        /// </summary>
        private const int MAX_CREATE_REQUEST_LIMIT = 10;

        private readonly IVlogRepository _vlogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;

        private readonly ILivestreamingService _livestreamingService;
        private readonly INotificationService _notificationService;

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
            //TODO Obtain and determine users to send a vlog request
            var targetedUsers = new List<User>()
            {
                // TODO Currently using only the example user (user@example.com) for testing purposes
                await _userRepository.GetByIdAsync(new Guid("d206ad6c-0bdc-4f17-903a-3b5c260de8c2"))
            };

            //TODO  For each user that should be triggered:
            foreach (User user in targetedUsers)
            {
                try
                {
                    // Reserve a livestream
                    StreamConnectionDetails connectionDetails = await _livestreamingService.ReserveLiveStreamForUserAsync(user.UserId);

                    // Send notification to this user with the livestream connection details
                    var notification = new SwabbrNotification
                    {
                        MessageContent = new SwabbrNotificationBody
                        {
                            Title = "Time to record a vlog!",
                            Body = "It's time to record a vlog!",
                            Object = JObject.FromObject(connectionDetails),
                            ObjectType = typeof(StreamConnectionDetails).Name,
                            ClickAction = ClickActions.VLOG_RECORD_REQUEST
                        }
                    };

                    await _livestreamingService.StartStreamAsync(connectionDetails.Id);
                    System.Diagnostics.Debug.WriteLine($"Started livestream for user {user.FirstName} livestream id: {connectionDetails.Id}. Sending notification in 1 minute.");

                    // Wait before sending the notification to ensure the stream has started. TODO
                    // Instead of waiting x minutes, we could also poll the state of the livestream
                    // until it is 'started' here.
                    await Task.Delay(TIMESPAN_START_STREAM);

                    // Send the notification containing the stream connection details to the user.
                    await _notificationService.SendNotificationToUserAsync(notification, user.UserId);

                    // Wait for time-out
                    _ = WaitForTimeoutAsync(user.UserId, connectionDetails.Id);
                }
                catch (Exception e)
                {
                    //TODO Handle exception

                    // If we get here, either no stream could be reserved, something went wrong with
                    // receiving hub registrations (in which case we can not send any notifications)
                    // or something went wrong while sending out a notification (in which case we
                    // should possibly try to send it again)
                }
            }

            // Make sure enough livestreams are available for usage
            int availableStreamCount = await _livestreamRepository.GetAvailableLivestreamCountAsync();
            int createdCount = 0;

            // Replenish livestreams
            try
            {
                // Try to create new streams to fill up the available streams to the target amount
                while (createdCount < (MAX_STREAM_POOL_SIZE - availableStreamCount) &&
                        createdCount <= MAX_CREATE_REQUEST_LIMIT)
                {
                    await _livestreamingService.CreateNewStreamAsync("test");
                    createdCount++;
                }
            }
            catch (ExternalErrorException e)
            {
                //TODO Handle exception. Livestream request limit reached or could not create livestream
                System.Diagnostics.Debug.WriteLine($"Exception thrown during pool replenishment ({e.Message}).");
            }
        }

        public async Task WaitForTimeoutAsync(Guid userId, string livestreamId)
        {
            // Wait 20 minutes
            await Task.Delay(TIMESPAN_TIMEOUT);

            System.Diagnostics.Debug.WriteLine($"Livestream {livestreamId} has timed out.");

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            //TODO For later:
            /*
                             * If a user is currently streaming at this point (livestream has just started),
                             * extend the delay before deactivating the stream?
                             */

            try
            {
                // Ensure the livestream is stopped and deleted from the service.
                await _livestreamingService.StopStreamAsync(livestreamId);

                //TODO Dispose of livestream externally as well as internally. Currently keeping it as active because recordings are not being stored internally.
                ////await _livestreamingService.DeleteStreamAsync(livestreamId);
                ////await _livestreamRepository.DeleteAsync(livestream);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Exception thrown in timeout. Message: {e.Message}");
            }
        }

        private async Task<bool> ShouldUserRecordVlog(Guid userId)
        {
            // TODO Determine whether the user is elligible for reserving a livestream (recording a vlog)

            User user = await _userRepository.GetByIdAsync(userId);

            // TODO Currently determining this arbitrarily for testing purposes.
            return user.Email.Equals("user@example.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Start the interval
            _timer = new Timer(OnTriggerEventAsync, null, TimeSpan.Zero, TIMESPAN_INTERVAL);
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

        #endregion IDisposable Support
    }
}