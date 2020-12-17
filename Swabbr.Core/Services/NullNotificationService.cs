using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Notification service which does nothing.
    /// </summary>
    /// <remarks>
    ///     This never validates any parameters.
    /// </remarks>
    public class NullNotificationService : INotificationService
    {
        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task NotifyFollowersVlogPostedAsync(Guid userId, Guid vlogId) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task NotifyReactionPlacedAsync(Guid receivingUserId, Guid vlogId, Guid reactionId) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task NotifyVlogLikedAsync(Guid receivingUserId, VlogLikeId vlogLikeId) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task NotifyVlogRecordRequestAsync(Guid userId, Guid vlogId, TimeSpan requestTimeout) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task RegisterAsync(Guid userId, PushNotificationPlatform platform, string handle) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>.
        /// </summary>
        public Task UnregisterAsync(Guid userId) => Task.CompletedTask;
    }
}
