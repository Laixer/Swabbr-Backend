﻿using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     A contract for a service to handle everything regarding notification.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        ///     Send a vlog record request notification.
        /// </summary>
        /// <param name="userId">User id to notify.</param>
        /// <param name="vlogId">The suggested vlog id to post.</param>
        /// <param name="requestTimeout">The timeout time span for the request.</param>
        Task NotifyVlogRecordRequestAsync(Guid userId, Guid vlogId, TimeSpan requestTimeout);

        /// <summary>
        ///     Notify all followers of a user that a new vlog was posted.
        /// </summary>
        /// <param name="vlogOwnerUserId">Owner of the vlog.</param>
        /// <param name="vlogId">The posted vlog id.</param>
        Task NotifyFollowersVlogPostedAsync(Guid vlogOwnerUserId, Guid vlogId);

        /// <summary>
        ///     Notify a user that a reaction was placed on one of 
        ///     the users own vlogs.
        /// </summary>
        /// <param name="receivingUserId">User that received the reaction.</param>
        /// <param name="vlogId">The id of the vlog.</param>
        /// <param name="reactionId">The placed reaction id.</param>
        Task NotifyReactionPlacedAsync(Guid receivingUserId, Guid vlogId, Guid reactionId);

        /// <summary>
        ///     Notify a user that one of the users vlogs received
        ///     a new like.
        /// </summary>
        /// <param name="receivingUserId">User that received the reaction.</param>
        /// <param name="vlogLikeId">The vlog like id.</param>
        Task NotifyVlogLikedAsync(Guid receivingUserId, VlogLikeId vlogLikeId);

        /// <summary>
        ///     Registers a device to our notification services.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="platform">The user platform.</param>
        /// <param name="handle">The device handle.</param>
        Task RegisterAsync(Guid userId, PushNotificationPlatform platform, string handle);

        /// <summary>
        ///     Unregisters a user from our notification services.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task UnregisterAsync(Guid userId);
    }
}
