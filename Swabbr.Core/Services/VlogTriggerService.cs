﻿using Laixer.Utility.Exceptions;
using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Handles the triggers where a <see cref="SwabbrUser"/> has to start vlogging.
    /// This fires the AMS livestream services and the ANH notification services:
    /// - <see cref="ILivestreamService"/>
    /// - <see cref="INotificationService"/>
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
        /// Process the external trigger that a <see cref="SwabbrUser"/> has to start
        /// vlogging.
        /// </summary>
        /// <remarks>
        /// TODO This should poll after the request was received, and only then start the timer (not for version 1)
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTriggerForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            try
            {
                logger.LogTrace($"{nameof(ProcessVlogTriggerForUserAsync)} - Attempting vlog trigger for user {userId}");

                userId.ThrowIfNullOrEmpty();
                triggerMinute.ThrowIfNullOrEmpty();

                // Internal checks
                if (!await _userService.ExistsAsync(userId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(SwabbrUser)); }

                // Process livestream
                var livestream = await _livestreamingService.TryClaimLivestreamForUserAsync(userId, triggerMinute).ConfigureAwait(false);

                // Process notifications
                var parameters = await _livestreamingService.GetParametersRecordVlogAsync(livestream.Id, triggerMinute).ConfigureAwait(false);
                await _notificationService.NotifyVlogRecordRequestAsync(userId, livestream.Id, parameters).ConfigureAwait(false);

                logger.LogTrace($"{nameof(ProcessVlogTriggerForUserAsync)} - Completed vlog trigger for user {userId}");
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(ProcessVlogTriggerForUserAsync)} - Exception while calling ", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Processes the timeout for a vlog request for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTimeoutForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            try
            {
                logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Attempting vlog timeout for user {userId}");

                userId.ThrowIfNullOrEmpty();
                triggerMinute.ThrowIfNullOrEmpty();
                if (!await _userService.ExistsAsync(userId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(userId)); }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var livestream = await _livestreamingService.GetLivestreamFromTriggerMinute(userId, triggerMinute).ConfigureAwait(false) as Livestream;

                    switch (livestream.LivestreamStatus)
                    {
                        // This means we have to perform a timeout - user did not respond in time
                        case LivestreamStatus.PendingUser:
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Processing timeout for user {userId}");
                            await _livestreamingService.ProcessTimeoutAsync(userId, livestream.Id).ConfigureAwait(false);
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - Processed timeout for user {userId}");
                            scope.Complete();
                            return;

                        // No need for request timeout processing
                        case LivestreamStatus.Created | LivestreamStatus.PendingUserConnect | LivestreamStatus.Live | LivestreamStatus.PendingClosure | LivestreamStatus.Closed:
                            logger.LogTrace($"{nameof(ProcessVlogTimeoutForUserAsync)} - No vlog timeout actions required for for user {userId}");
                            return;

                        // Should never happen
                        case LivestreamStatus.UserNoResponseTimeout:
                            throw new LivestreamStateException($"Timeout trigger found livestream already marked as {LivestreamStatus.UserNoResponseTimeout.GetEnumMemberAttribute()}");

                        // Should never happen
                        case LivestreamStatus.CreatedInternal:
                            throw new LivestreamStateException($"Timeout trigger found livestream marked as {LivestreamStatus.CreatedInternal.GetEnumMemberAttribute()}");
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
