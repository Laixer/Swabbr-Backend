using Laixer.Utility.Exceptions;
using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Handles the triggers where a <see cref="SwabbrUser"/> has to stat vlogging.
    /// </summary>
    public sealed class VlogTriggerService : IVlogTriggerService
    {

        private readonly ILivestreamService _livestreamingService;
        private readonly IUserService _userService;
        private readonly IHashDistributionService _hashDistributionService;
        private readonly IRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger logger;
        private readonly SwabbrConfiguration config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerService(ILivestreamService livestreamingService,
            IUserService userService,
            IHashDistributionService hashDistributionService,
            IRequestRepository requestRepository,
            INotificationService notificationService,
            ILoggerFactory loggerFactory,
            IOptions<SwabbrConfiguration> options)
        {
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _hashDistributionService = hashDistributionService ?? throw new ArgumentNullException(nameof(hashDistributionService));
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(VlogTriggerService)) : throw new ArgumentNullException(nameof(loggerFactory));

            if (options == null || options.Value == null) { throw new ConfigurationException($"{nameof(SwabbrConfiguration)} is not configured"); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
        }

        /// <summary>
        /// Processes all vlog trigger requests for a given minute in 
        /// <paramref name="time"/>.
        /// </summary>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTriggersAsync(DateTimeOffset time)
        {
            if (time == null) { throw new ArgumentNullException(nameof(time)); }

            var allMinifiedUsers = await _userService.GetAllVloggableUserMinifiedAsync().ConfigureAwait(false);
            var minifiedUsers = _hashDistributionService.GetForMinute(allMinifiedUsers, time);

            foreach (var user in minifiedUsers)
            {
                await ProcessVlogTriggerForUserAsync(user.Id, time).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Process the external trigger that a <see cref="SwabbrUser"/> has to start
        /// vlogging.
        /// </summary>
        /// <remarks>
        /// TODO This should poll after the request was received, and only then start the timer
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTriggerForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            try
            {
                logger.LogTrace($"{nameof(ProcessVlogTriggerForUserAsync)} - Attempting vlog trigger for user {userId}");

                userId.ThrowIfNullOrEmpty();
                if (!await _userService.ExistsAsync(userId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(userId)); }

                if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }


                var livestream = await _livestreamingService.TryStartLivestreamForUserAsync(userId, triggerMinute).ConfigureAwait(false);

                // TODO This should poll after the request was received, then start the timer
                var parameters = await _livestreamingService.GetUpstreamParametersAsync(livestream.Id, userId).ConfigureAwait(false);
                await _notificationService.NotifyVlogRecordRequestAsync(userId, livestream.Id, parameters).ConfigureAwait(false);

                // Debug log all that we just determined
                logger.LogDebug($@"{nameof(ProcessVlogTriggerForUserAsync)} - Parameters: 
                    \tApplicationName = {parameters.ApplicationName}\n
                    \tHostPort = {parameters.HostPort}\n
                    \tHostServer = {parameters.HostServer}\n
                    \tLivestreamId = {parameters.LivestreamId}\n
                    \tPassword = {parameters.Password}\n,
                    \tStreamKey = {parameters.StreamKey}\n
                    \tUsername = {parameters.Username}\n");

                logger.LogTrace($"{nameof(ProcessVlogTriggerForUserAsync)} - Completed vlog trigger for user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(ProcessVlogTriggerForUserAsync)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Process all vlog timeouts for a given minute on a day.
        /// </summary>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTimeoutsAsync(DateTimeOffset time)
        {
            if (time == null) { throw new ArgumentNullException(nameof(time)); }

            var allMinifiedUsers = await _userService.GetAllVloggableUserMinifiedAsync().ConfigureAwait(false);
            var originalTriggerMinute = time.Subtract(TimeSpan.FromMinutes(config.VlogRequestTimeoutMinutes));
            var minifiedUsers = _hashDistributionService.GetForMinute(allMinifiedUsers, time, TimeSpan.FromMinutes(config.VlogRequestTimeoutMinutes));

            foreach (var user in minifiedUsers)
            {
                await ProcessVlogTimeoutForUserAsync(user.Id, originalTriggerMinute).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the timeout for a vlog request for a <paramref name="userId"/>.
        /// </summary>
        /// <remarks>
        /// TODO What to do with requests that weren't sent yet?
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTimeoutForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            try
            {
                logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Attempting vlog timeout for user {userId}");

                userId.ThrowIfNullOrEmpty();
                if (!await _userService.ExistsAsync(userId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(userId)); }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var livestream = await _livestreamingService.GetLivestreamFromTriggerMinute(userId, triggerMinute).ConfigureAwait(false) as Livestream;

                    switch (livestream.LivestreamStatus)
                    {
                        // TODO This is a race condition
                        case LivestreamStatus.Created:
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Timeout race condition encountered for user {userId} with status = {LivestreamStatus.Created.GetEnumMemberAttribute()} - doing nothing");
                            break;

                        // This means we have to perform a timeout - user did not respond in time
                        case LivestreamStatus.PendingUser:
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Processing timeout for user {userId}");
                            await _livestreamingService.ProcessTimeoutAsync(userId, livestream.Id).ConfigureAwait(false);
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Processed timeout for user {userId}");
                            scope.Complete();
                            return;

                        // No need for request timeout processing
                        case LivestreamStatus.Live | LivestreamStatus.PendingClosure | LivestreamStatus.Closed:
                            return;

                        // Should never happen
                        case LivestreamStatus.UserNoResponseTimeout:
                            throw new InvalidOperationException($"Timeout trigger found livestream already marked as {LivestreamStatus.UserNoResponseTimeout.GetEnumMemberAttribute()}");
                    }

                    throw new InvalidOperationException(nameof(livestream.LivestreamStatus));
                }
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(ProcessVlogTimeoutForUserAsync)} - Exception while calling ", e.Message);
                throw;
            }
        }

    }

}
