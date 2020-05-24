using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Parameters for a <see cref="NotificationAction.VlogNewReaction"/> request.
    /// </summary>
    public sealed class ParametersVlogNewReaction : ParametersJsonBase
    {

        public Guid VlogId { get; set; }

        public Guid ReactionId { get; set; }

    }

}
