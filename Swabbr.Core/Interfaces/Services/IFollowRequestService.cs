using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for processing follow request related operations.
    /// </summary>
    public interface IFollowRequestService
    {
        /// <summary>
        ///     Accept an existing follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        Task AcceptAsync(FollowRequestId id);

        /// <summary>
        ///     Cancel an existing follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        Task CancelAsync(FollowRequestId id);

        /// <summary>
        ///     Decline an existing follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        Task DeclineAsync(FollowRequestId id);

        /// <summary>
        ///     Gets a follow request from our data store.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <returns>Follow request entity.</returns>
        Task<FollowRequest> GetAsync(FollowRequestId id);

        /// <summary>
        ///     Gets the <see cref="FollowRequestStatus"/> for a 
        ///     given <see cref="FollowRequest"/> between two users.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <returns><see cref="FollowRequestStatus"/>.</returns>
        Task<FollowRequestStatus> GetStatusAsync(FollowRequestId id);

        /// <summary>
        ///     Returns all pending incoming follow requests for a specific user.
        /// </summary>
        /// <param name="userId">Id of user that receives the requests.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        IAsyncEnumerable<FollowRequest> GetPendingIncomingForUserAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Returns all pending outgoing follow requests from a specific user.
        /// </summary>
        /// <param name="userId">Id of user that sent out the requests.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        IAsyncEnumerable<FollowRequest> GetPendingOutgoingForUserAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Returns the amount of users that follow the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that is being followed.</param>
        /// <returns>The amount of followers.</returns>
        Task<uint> GetFollowerCountAsync(Guid userId);

        /// <summary>
        ///     Returns the amount of users that the specified user is following.
        /// </summary>
        /// <param name="userId">Id of user to check the amount of followers for.</param>
        /// <returns>User following count.</returns>
        Task<uint> GetFollowingCountAsync(Guid userId);

        /// <summary>
        ///     Send a follow request from a requesting user to a receiving user.
        /// </summary>
        /// <param name="requesterId">User id that follows.</param>
        /// <param name="receiverId">User id that is being followed.</param>
        /// <returns>The created follow request id.</returns>
        Task<FollowRequestId> SendAsync(Guid requesterId, Guid receiverId);

        /// <summary>
        ///     Unfollows a requester from a receiver.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        Task UnfollowAsync(FollowRequestId id);
    }
}
