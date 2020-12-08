using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for processing follow request related operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever possible,
    ///     this id should extracted from the context.
    /// </remarks>
    public interface IFollowRequestService
    {
        /// <summary>
        ///     Accept an existing follow request.
        /// </summary>
        /// <param name="requesterId">The user that will be following.</param>
        Task AcceptAsync(Guid requesterId);

        /// <summary>
        ///     Cancel an existing follow request.
        /// </summary>
        /// <param name="receiverId">The user that would have been followed.</param>
        Task CancelAsync(Guid receiverId);

        /// <summary>
        ///     Decline an existing follow request.
        /// </summary>
        /// <param name="requesterId">The user that would be following.</param>
        Task DeclineAsync(Guid requesterId);

        /// <summary>
        ///     Gets a follow request from our data store.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <returns>Follow request entity.</returns>
        Task<FollowRequest> GetAsync(FollowRequestId id);

        /// <summary>
        ///     Gets the status of a follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <returns>The follow request status.</returns>
        Task<FollowRequestStatus> GetStatusAsync(FollowRequestId id);

        /// <summary>
        ///     Returns all pending incoming follow requests for
        ///     the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        IAsyncEnumerable<FollowRequest> GetPendingIncomingForUserAsync(Navigation navigation);

        /// <summary>
        ///     Returns all pending outgoing follow requests
        ///     for the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        IAsyncEnumerable<FollowRequest> GetPendingOutgoingForUserAsync(Navigation navigation);

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
        ///     Send a follow request from the current user to a receiving user.
        /// </summary>
        /// <param name="receiverId">User id that is being followed.</param>
        /// <returns>The created follow request id.</returns>
        Task<FollowRequestId> SendAsync(Guid receiverId);

        /// <summary>
        ///     Unfollows the current user from a specified user.
        /// </summary>
        /// <param name="receiverId">The user that will be unfollowed.</param>
        Task UnfollowAsync(Guid receiverId);
    }
}
