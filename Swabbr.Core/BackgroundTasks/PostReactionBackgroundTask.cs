using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Context;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.BackgroundTasks
{
    /// <summary>
    ///     Background task to process the posting of a reaction.
    /// </summary>
    public class PostReactionBackgroundTask : BackgroundTask
    {
        private readonly IReactionService _reactionService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PostReactionBackgroundTask(IReactionService reactionService)
            => _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));

        /// <summary>
        ///     Checks if we can handle a given object.
        /// </summary>
        /// <remarks>
        ///     A <see cref="PostReactionContext"/> is expected.
        /// </remarks>
        /// <param name="value">The object to check.</param>
        public override bool CanHandle(object value)
            => value is PostReactionContext;

        /// <summary>
        ///     Fires <see cref="IVlogService.PostVlogAsync(PostVlogContext)"/>.
        /// </summary>
        /// <param name="context">Background task context.</param>
        public override async Task ExecuteAsync(BackgroundTaskContext context)
        {
            if (context is null) 
            { 
                throw new ArgumentNullException(nameof(context)); 
            }
            if (!CanHandle(context.Value))
            {
                throw new InvalidOperationException();
            }

            var postReactionContext = context.Value as PostReactionContext;

            await _reactionService.PostReactionAsync(postReactionContext);
        }
    }
}
