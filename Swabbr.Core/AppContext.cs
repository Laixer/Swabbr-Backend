using System;
using System.Threading;

namespace Swabbr.Core
{
    /// <summary>
    ///     Application context.
    /// </summary>
    public class AppContext
    {
        /// <summary>
        ///     Used to signal when an operation should be aborted.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        ///     Service provider for the service container.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        ///     User id that belongs to the context.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Checks if we have a signed in user.
        /// </summary>
        public bool HasUser => UserId != Guid.Empty;

        /// <summary>
        ///     Checks if <see cref="UserId"/> is set and
        ///     matches <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user id to check.</param>
        public bool IsUser(Guid userId)
            => HasUser && UserId == userId;
    }
}
