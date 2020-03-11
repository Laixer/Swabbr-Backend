using Laixer.Utility.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
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

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerService(ILivestreamService livestreamingService,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// Process the external trigger that a <see cref="SwabbrUser"/> has to start
        /// vlogging.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessVlogTriggerForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            if (!await _userRepository.UserExistsAsync(userId).ConfigureAwait(false)) { throw new InvalidOperationException("User doesn't exist"); }

            var livestream = await _livestreamingService.TryStartLivestreamForUserAsync(userId).ConfigureAwait(false);

            TriggerUserTimeoutFunction(); // TODO HOW?
            TriggerLivestreamTimeoutFunction(); // TODO HOW?

            var parameters = await _livestreamingService.GetUpstreamParametersAsync(livestream.Id, userId).ConfigureAwait(false);
            await _notificationService.VlogRecordRequestAsync(userId, livestream.Id, parameters).ConfigureAwait(false);
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
