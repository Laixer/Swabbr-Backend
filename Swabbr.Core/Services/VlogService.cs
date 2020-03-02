using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Contains request processing functionality for <see cref="Vlog"/>s and
    /// <see cref="VlogLike"/>s.
    /// </summary>
    public class VlogService : IVlogService
    {

        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogService(IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IUserRepository userRepository)
        {
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
        }

        /// <summary>
        /// Deletes a <see cref="Vlog"/> from our data store.
        /// </summary>
        /// <remarks>
        /// Throws a <see cref="NotAllowedException"/> if our <paramref name="userId"/>
        /// does not own the <paramref name="vlogId"/>.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await _userRepository.GetAsync(userId).ConfigureAwait(false);
                var vlog = await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false);
                if (vlog.UserId != user.Id) { throw new NotAllowedException("User does not own vlog"); }

                await _vlogRepository.DeleteAsync(vlogId).ConfigureAwait(false);
                scope.Complete();
            }
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> from our data store.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Vlog"/></returns>
        public Task<Vlog> GetAsync(Guid vlogId)
        {
            return _vlogRepository.GetAsync(vlogId);
        }

        /// <summary>
        /// Gets all <see cref="VlogLike"/> entities that belong to a given
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public Task<IEnumerable<VlogLike>> GetVlogLikesForVlogAsync(Guid vlogId)
        {
            return _vlogLikeRepository.GetForVlogAsync(vlogId);
        }

        /// <summary>
        /// Creates a <see cref="VlogLike"/> between a <see cref="Vlog"/>
        /// and a <see cref="SwabbrUser"/>.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="OperationAlreadyExecutedException"/> if the user
        /// already liked the vlog.
        /// 
        /// Throws an <see cref="NotAllowedException"/> if the user owns the vlog.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task LikeAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var vlogLikeId = new VlogLikeId { VlogId = vlogId, UserId = userId };
                if (await _vlogLikeRepository.ExistsAsync(vlogLikeId).ConfigureAwait(false)) { throw new OperationAlreadyExecutedException("User already liked given vlog"); }
                if ((await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false)).UserId == userId) { throw new NotAllowedException("Can't like your own vlog"); }

                await _vlogLikeRepository.CreateAsync(new VlogLike
                {
                    Id = vlogLikeId
                }).ConfigureAwait(false);
                 
                scope.Complete();
            }
        }

        /// <summary>
        /// Lists all <see cref="Vlog"/> entities that are owned by a given
        /// <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Vlog"/> collection</returns>
        public Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId)
        {
            return _vlogRepository.GetVlogsFromUserAsync(userId);
        }

        /// <summary>
        /// Unlikes a <see cref="Vlog"/> by deleting the corresponding 
        /// <see cref="VlogLike"/>.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task UnlikeAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var vlogLikeId = new VlogLikeId
                {
                    VlogId = vlogId,
                    UserId = userId
                };
                if (!await _vlogLikeRepository.ExistsAsync(vlogLikeId).ConfigureAwait(false)) { throw new EntityNotFoundException(); }

                await _vlogLikeRepository.DeleteAsync(vlogLikeId).ConfigureAwait(false);

                scope.Complete();
            }
        }

        /// <summary>
        /// Updates a <see cref="Vlog"/> in our data store.
        /// TODO Is this optimal?
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Updated and queried <see cref="Vlog"/></returns>
        public async Task<Vlog> UpdateAsync(Guid vlogId, Guid userId, bool isPrivate)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await _userRepository.GetAsync(userId).ConfigureAwait(false);
                var vlog = await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false);
                if (vlog.UserId != user.Id) { throw new NotAllowedException("User doesn't own vlog"); }

                vlog.IsPrivate = isPrivate;
                // TODO Implement shared users here

                var updatedVlog = await _vlogRepository.UpdateAsync(vlog).ConfigureAwait(false);

                // Commit and return
                scope.Complete();
                return updatedVlog;
            }
        }

    }

}
