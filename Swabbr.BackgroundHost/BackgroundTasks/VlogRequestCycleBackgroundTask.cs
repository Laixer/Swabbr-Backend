using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.BackgroundHost.BackgroundTasks
{
    /// <summary>
    ///     Background task representing a vlog request cycle,
    ///     sending a vlog record request to each user based on
    ///     a given minute of the day.
    /// </summary>
    public class VlogRequestCycleBackgroundTask : BackgroundTask
    {
        private readonly IVlogRequestService _vlogRequestService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <param name="vlogRequestService"></param>
        public VlogRequestCycleBackgroundTask(IVlogRequestService vlogRequestService)
            => _vlogRequestService = vlogRequestService ?? throw new ArgumentNullException(nameof(vlogRequestService));

        /// <summary>
        ///     Checks if we can handle a given object.
        /// </summary>
        /// <param name="value">The object to check.</param>
        public override bool CanHandle(object value)
            => value is DateTimeOffset;
        
        /// <summary>
        ///     Send a vlog record request to all selected
        ///     users according to the current time of day.
        /// </summary>
        /// <param name="context">Background task context.</param>
        public override async Task ExecuteAsync(BackgroundTaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Value is not DateTimeOffset)
            {
                throw new InvalidOperationException(nameof(context.Value));
            }

            var dto = (DateTimeOffset)context.Value;
            var minute = new DateTimeOffset(dto.Year, dto.Month, dto.Day, dto.Hour, dto.Minute, 0, TimeSpan.Zero);

            await _vlogRequestService.SendVlogRequestsForMinuteAsync(minute);
        }
    }
}
