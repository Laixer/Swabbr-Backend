using Laixer.Utility.Exceptions;
using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Handles the triggers where a <see cref="SwabbrUser"/> has to stat vlogging.
    /// </summary>
    public sealed class VlogTriggerService : IVlogTriggerService
    {

        private readonly ILivestreamService _livestreamingService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger logger;
        private readonly SwabbrConfiguration swabbrConfiguration;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerService(ILivestreamService livestreamingService,
            IUserRepository userRepository,
            INotificationService notificationService,
            ILoggerFactory loggerFactory,
            IOptions<SwabbrConfiguration> options)
        {
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(VlogTriggerService)) : throw new ArgumentNullException(nameof(loggerFactory));

            if (options == null || options.Value == null) { throw new ConfigurationException($"{nameof(SwabbrConfiguration)} is not configured"); }
            options.Value.ThrowIfInvalid();
            swabbrConfiguration = options.Value;
        }

        /// <summary>
        /// Process the external trigger that a <see cref="SwabbrUser"/> has to start
        /// vlogging.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTriggerForUserAsync(Guid userId)
        {
            try
            {
                logger.LogTrace($"{nameof(ProcessVlogTriggerForUserAsync)} - Attempting vlog trigger for user {userId}");
                userId.ThrowIfNullOrEmpty();
                if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new InvalidOperationException("User doesn't exist"); }

                var livestream = await _livestreamingService.TryStartLivestreamForUserAsync(userId).ConfigureAwait(false);

                TriggerUserTimeoutFunction(); // TODO HOW?
                TriggerLivestreamTimeoutFunction(); // TODO HOW?

                var parameters = await _livestreamingService.GetUpstreamParametersAsync(livestream.Id, userId).ConfigureAwait(false);

                // TODO This should poll after the request was received, 
                // Then start the timer
                int i = 0;
                await _notificationService.VlogRecordRequestAsync(userId, livestream.Id, parameters).ConfigureAwait(false);

                // TODO Might want to remove this?
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

        public Task ProcessVlogTriggerTimoutAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This serves as a timeout in case the user never responds.
        /// </summary>
        private void TriggerUserTimeoutFunction()
        {
            return;
        }

        /// <summary>
        /// This serves as a timeout in case the vlog goes on for too long.
        /// TODO Do we even need this?
        /// </summary>
        private void TriggerLivestreamTimeoutFunction()
        {
            return;
        }

    }

}
