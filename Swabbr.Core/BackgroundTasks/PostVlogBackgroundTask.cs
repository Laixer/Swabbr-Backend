using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Context;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.BackgroundTasks
{
    /// <summary>
    ///     Background task to process the posting of a vlog.
    /// </summary>
    public class PostVlogBackgroundTask : BackgroundTask
    {
        private readonly IVlogService _vlogService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PostVlogBackgroundTask(IVlogService vlogService)
            => _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));

        /// <summary>
        ///     Checks if we can handle a given object.
        /// </summary>
        /// <remarks>
        ///     A <see cref="PostVlogContext"/> is expected.
        /// </remarks>
        /// <param name="value">The object to check.</param>
        public override bool CanHandle(object value)
            => value is PostVlogContext;

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

            var postVlogContext = context.Value as PostVlogContext;

            await _vlogService.PostVlogAsync(postVlogContext);
        }
    }
}
