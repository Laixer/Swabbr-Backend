using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a follow request repository.
    /// </summary>
    public interface IFollowRequestRepository : IRepository<FollowRequest, FollowRequestId>
    {
        /// <summary>
        ///     Gets the total amoun tof users that are 
        ///     following <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The follower count.</returns>
        Task<uint> GetFollowerCountAsync(Guid userId);

        /// <summary>
        ///     Gets the total amount of users that the 
        ///     <paramref name="userId"/> is following.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Total following count.</returns>
        Task<uint> GetFollowingCountAsync(Guid userId);

        /// <summary>
        ///     Gets incoming follow requests for a user.
        /// </summary>
        /// <param name="userId">The user that will be followed.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Incoming follow requests.</returns>
        IAsyncEnumerable<FollowRequest> GetIncomingForUserAsync(Guid userId, Navigation navigation);
        
        /// <summary>
        ///     Gets outgoing follow requests for a user.
        /// </summary>
        /// <param name="userId">The user that will follow.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Outgoing follow requests.</returns>
        IAsyncEnumerable<FollowRequest> GetOutgoingForUserAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Updates the status of a follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <param name="status">The new status.</param>
        Task UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status);
    }
}
