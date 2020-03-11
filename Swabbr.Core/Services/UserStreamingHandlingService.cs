using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Contains functionality to handle user streaming requests.
    /// </summary>
    public sealed class UserStreamingHandlingService : IUserStreamingHandlingService
    {

        private readonly ILivestreamService _livestreamingService;
        private readonly INotificationService _notificationService;

        private readonly IUserRepository _userRepository;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogRepository _vlogRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserStreamingHandlingService(ILivestreamService livestreamingService,
            INotificationService notificationService,
            IUserRepository userRepository,
            ILivestreamRepository livestreamRepository,
            IVlogRepository vlogRepository)
        {
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
        }

        /// <summary>
        /// Called when the user starts streaming to a <see cref="Livestream"/>.
        /// This creates a <see cref="Vlog"/> to bind to the stream.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns>The created <see cref="Vlog"/></returns>
        public async Task<Vlog> OnUserStartStreaming(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await _userRepository.GetAsync(userId).ConfigureAwait(false);
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

                // Throw if the user doesn't own the livestream
                if (livestream.UserId != user.Id) { throw new InvalidOperationException("User doesn't own this livestream"); }

                // Throw if a vlog already exists for this livestream
                if (await _vlogRepository.ExistsForLivestreamAsync(livestreamId).ConfigureAwait(false)) { throw new InvalidOperationException("This livestream already contains a vlog."); }

                // Create a new vlog for the livestream
                var vlog = await _vlogRepository.CreateAsync(new Vlog
                {
                    UserId = user.Id,
                    LivestreamId = livestreamId,
                }).ConfigureAwait(false);

                // Process livestream itself
                // This throws if the livestream isn't pending user
                await _livestreamingService.OnUserStartStreamingAsync(livestream.Id, user.Id).ConfigureAwait(false);

                // Notify all followers
                await _notificationService.NotifyFollowersProfileLiveAsync(user.Id, livestream.Id).ConfigureAwait(false);

                // Commit and return
                scope.Complete();
                return vlog;
            }
        }

        /// <summary>
        /// Gets called when the user stops streaming.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnUserStopStreaming(Guid userId, Guid livestreamId)
        {
            userId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await _userRepository.GetAsync(userId).ConfigureAwait(false);
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

                // Throw if the user doesn't own the livestream
                if (livestream.UserId != user.Id) { throw new InvalidOperationException("User doesn't own this livestream"); }

                // Stop the livestream
                // This throws if the livestream isn't live
                // This throws if the user isn't actually connected at first
                await _livestreamingService.OnUserStopStreamingAsync(livestream.Id, userId).ConfigureAwait(false);

                // Notify all followers
                var vlog = await _vlogRepository.GetVlogFromLivestreamAsync(livestream.Id).ConfigureAwait(false);
                // await _notificationService.SendNotificationToFollowersAsync(user.Id, vlog.Id).ConfigureAwait(false);
                throw new NotImplementedException();

                scope.Complete();
            }
        }

    }

}
