using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    ///     Contract for sending and timeout-processing
    ///     vlog record requests.
    /// </summary>
    public interface IVlogRequestService
    {
        /// <summary>
        ///     Should be called when a vlog request timed out.
        /// </summary>
        /// <param name="userId">The user of the vlog request.</param>
        Task OnVlogRequestTimedOutAsync(Guid userId);

        /// <summary>
        ///     Get all users that should receive a vlog request for
        ///     the specified <paramref name="minute"/>. Each will
        ///     be sent a vlog record request.
        /// </summary>
        /// <remarks>
        ///     The minute does not take timezones into account. This
        ///     should be handled in the implementation itself.
        /// </remarks>
        /// <param name="minute">The minute to check.</param>
        Task SendVlogRequestsForMinuteAsync(DateTimeOffset minute);

        /// <summary>
        ///     Process a vlog request for a single user. This will
        ///     claim a livestream and then notify the user. A timeout
        ///     will be started.
        /// </summary>
        /// <param name="userId">The user that should vlog.</param>
        Task SendVlogRequestToUserAsync(Guid userId);
    }
}
