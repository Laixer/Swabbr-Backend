using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Handles vlog requests.
    /// </summary>
    public class VlogRequestService : IVlogRequestService
    {
        protected readonly INotificationService _notificationService; // TODO To queue
        protected readonly IUserSelectionService _userSelectionService;
        protected readonly SwabbrConfiguration _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogRequestService(INotificationService notificationService,
            IUserSelectionService userSelectionService,
            IOptions<SwabbrConfiguration> options)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userSelectionService = userSelectionService ?? throw new ArgumentNullException(nameof(userSelectionService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     Process a vlog request for a single user.
        /// </summary>
        /// <param name="userId">The user that should vlog.</param>
        public async Task SendVlogRequestToUserAsync(Guid userId)
        {
            // The vlog id that is created here does not exist in our data store
            // yet, but should be used by the client as the uploading file name.
            var vlogId = Guid.NewGuid();
            var requestTimeout = TimeSpan.FromMinutes(_options.VlogRequestTimeoutMinutes);

            // TODO Enqueue
            await _notificationService.NotifyVlogRecordRequestAsync(userId, vlogId, requestTimeout).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a vlog record request to each user that
        ///     should get a request in the given <paramref name="minute"/>.
        /// </summary>
        /// <param name="minute">The minute to check.</param>
        public async Task SendVlogRequestsForMinuteAsync(DateTimeOffset minute)
        {
            await foreach (var user in _userSelectionService.GetForMinuteAsync(minute))
            {
                // TODO Enqueue
                await SendVlogRequestToUserAsync(user.Id).ConfigureAwait(false);
            }
        }
    }
}
