using Swabbr.Core.Abstractions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Service that handles all vlog like related operations.
    /// </summary>
    public class VlogLikeService : AppServiceBase, IVlogLikeService
    {
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogLikeService(AppContext appContext,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            INotificationService notificationService)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        ///     Checks if a vlog like exists in our data store.
        /// </summary>
        /// <param name="vlogLikeId">The vlog like id to check.</param>
        public Task<bool> ExistsAsync(VlogLikeId vlogLikeId)
            => _vlogLikeRepository.ExistsAsync(vlogLikeId);

        /// <summary>
        ///     Gets a vlog like from our data store.
        /// </summary>
        /// <param name="vlogLikeId">The vlog like id.</param>
        /// <returns>The vlog like.</returns>
        public Task<VlogLike> GetAsync(VlogLikeId vlogLikeId)
            => _vlogLikeRepository.GetAsync(vlogLikeId);

        /// <summary>
        ///     Gets all the <see cref="VlogLike"/>s for a <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        ///     This does not scale. If that is required, use an implementation
        ///     of <see cref="GetVlogLikeSummaryForVlogAsync(Guid)"/> which does
        ///     not return all <see cref="VlogLike"/> but only a subset.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public IAsyncEnumerable<VlogLike> GetVlogLikesForVlogAsync(Guid vlogId, Navigation navigation)
            => _vlogLikeRepository.GetForVlogAsync(vlogId, navigation);

        /// <summary>
        ///     Gets a <see cref="VlogLikeSummary"/> for a given vlog.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.Users"/> does not need
        ///     to contain all the <see cref="User"/> users.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        public Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId)
            => _vlogLikeRepository.GetSummaryForVlogAsync(vlogId);

        /// <summary>
        ///     Used when the current users like a vlog. This also
        ///     dispatches a notification to the vlog owner.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        public async Task LikeAsync(Guid vlogId)
        {
            var vlogLikeId = await _vlogLikeRepository.CreateAsync(new VlogLike
            {
                Id = new VlogLikeId
                {
                    UserId = AppContext.UserId,
                    VlogId = vlogId
                }
            });

            var vlog = await _vlogRepository.GetAsync(vlogId);

            await _notificationService.NotifyVlogLikedAsync(vlog.UserId, vlogLikeId);
        }

        /// <summary>
        ///     Used when the current user unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        public Task UnlikeAsync(Guid vlogId)
            => _vlogLikeRepository.DeleteAsync(new VlogLikeId
            {
                VlogId = vlogId,
                UserId = AppContext.UserId
            });
    }
}
