using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    // FUTURE: Integrate this with a background task so we can fully utilize the IAsyncEnumerable.
    //         Right now we simply enqueue a lot of notification tasks.
    /// <summary>
    ///     Handles vlog requests.
    /// </summary>
    public class VlogRequestService : IVlogRequestService
    {
        protected readonly INotificationService _notificationService;
        protected readonly IUserSelectionService _userSelectionService;
        protected readonly SwabbrConfiguration _options;
        protected readonly ILogger<VlogRequestService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogRequestService(INotificationService notificationService,
            IUserSelectionService userSelectionService,
            IOptions<SwabbrConfiguration> options,
            ILogger<VlogRequestService> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userSelectionService = userSelectionService ?? throw new ArgumentNullException(nameof(userSelectionService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Process a vlog request for a single user.
        /// </summary>
        /// <param name="userId">The user that should vlog.</param>
        public virtual async Task SendVlogRequestToUserAsync(Guid userId)
        {
            // The vlog id that is generated here does not exist in our data store
            // yet, but should be used by the client as the uploading file name.
            var vlogId = Guid.NewGuid();
            var requestTimeout = TimeSpan.FromMinutes(_options.VlogRequestTimeoutMinutes);

            await _notificationService.NotifyVlogRecordRequestAsync(userId, vlogId, requestTimeout);
        }

        /// <summary>
        ///     Sends a vlog record request to each user that
        ///     should get a request in the given <paramref name="minute"/>.
        /// </summary>
        /// <param name="minute">The minute to check.</param>
        public virtual async Task SendVlogRequestsForMinuteAsync(DateTimeOffset minute)
        {
            _logger.LogTrace($"Sending vlog requests for minute {minute}");

            await foreach (var user in _userSelectionService.GetForMinuteAsync(minute))
            {
                await SendVlogRequestToUserAsync(user.Id);
            }
        }
    }
}
