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

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogRequestService(INotificationService notificationService,
            IUserSelectionService userSelectionService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userSelectionService = userSelectionService ?? throw new ArgumentNullException(nameof(userSelectionService));
        }

        /// <summary>
        ///     Claims a livestream for a user and sends
        ///     that user a vlog record request.
        /// </summary>
        /// <param name="userId">The user to request.</param>
        public async Task SendVlogRequestToUserAsync(Guid userId)
        {
            // TODO
            //var livestream = await _livestreamService.ClaimLivestreamForUserAsync(userId).ConfigureAwait(false);
            //var parameters = await _livestreamService.GetLivestreamParametersAsync(livestream.Id).ConfigureAwait(false);

            // TODO Enqueue
            //await _notificationService.NotifyVlogRecordRequestAsync(userId, livestream.Id, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a vlog record request to each user that
        ///     should get a request in the given <paramref name="minute"/>.
        /// </summary>
        /// <param name="minute">The minute to check.</param>
        public async Task SendVlogRequestsForMinuteAsync(DateTimeOffset minute)
        {
            var selectedUsers = await _userSelectionService.GetForMinuteAsync(minute).ConfigureAwait(false);

            foreach (var user in selectedUsers)
            {
                // TODO Enqueue
                await SendVlogRequestToUserAsync(user.Id).ConfigureAwait(false);
            }
        }

        // TODO
        public Task OnVlogRequestTimedOutAsync(Guid userId) 
            => throw new NotImplementedException();
    }
}
