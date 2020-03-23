using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.BackgroundWorkers;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.BackgroundWorkers
{

    /// <summary>
    /// Manages vlog trigger requests.
    /// </summary>
    public sealed class VlogTriggerWorker : IVlogTriggerWorker
    {

        private readonly ILogger logger;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IVlogTriggerService _vlogTriggerService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerWorker(ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IRequestRepository requestRepository,
            IVlogTriggerService vlogTriggerService)
        {
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(VlogTriggerWorker)) : throw new ArgumentNullException(nameof(loggerFactory));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
        }

        /// <summary>
        /// Sends all requests that need to be sent at this moment.
        /// </summary>
        /// <remarks>
        /// This function can take a very long time.
        /// </remarks>
        /// <returns><see cref="Task"/></returns>
        public async Task SendAllRequestsAsync()
        {
            try
            {
                var timeSpan = TimeSpan.FromDays(1);
                var from = DateTimeOffset.Now.Subtract(timeSpan);

                // First claim all users by creating a new request for each
                var userWithRequestCollection = new List<UserWithRequest>();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var users = (await _userRepository.GetVlogRequestableUsersAsync(from, timeSpan).ConfigureAwait(false)).ToList();
                    logger.LogInformation($"Sending vlog request to {users.Count} users");

                    foreach (var user in users)
                    {
                        // Create request
                        var request = await _requestRepository.CreateAsync(new RequestRecordVlog
                        {
                            RequestState = RequestState.Created,
                            UserId = user.Id
                        }).ConfigureAwait(false);

                        // Add to collection
                        userWithRequestCollection.Add(new UserWithRequest
                        {
                            User = user,
                            Request = request
                        });
                        logger.LogTrace($"Created non-sent request for user {user.Id}");
                    }

                    scope.Complete();
                    logger.LogTrace($"Created and committed non-sent request for {users.Count} users");
                }

                // Then send 
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var userWithRequest in userWithRequestCollection)
                    {
                        logger.LogTrace($"Marking and sending request for user {userWithRequest.User.Id}");
                        await _requestRepository.MarkAsync(userWithRequest.Request.Id, RequestState.Sent).ConfigureAwait(false);
                        await _vlogTriggerService.ProcessVlogTriggerForUserAsync(userWithRequest.User.Id).ConfigureAwait(false);
                        logger.LogTrace($"Marked and sent request for user {userWithRequest.User.Id}");
                    }
                    scope.Complete();
                    logger.LogTrace($"Marked and sent request for {userWithRequestCollection.Count} users");
                }

                logger.LogInformation($"Sent vlog request to {userWithRequestCollection.Count} users");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not process all vlog triggers.");
                throw new InvalidOperationException("VlogTriggerWorker could not process all vlog triggers");
            }
        }

    }

}
