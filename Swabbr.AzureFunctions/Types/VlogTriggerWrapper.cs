using System;

namespace Swabbr.AzureFunctions.Types
{

    /// <summary>
    /// Wrapper containing details about a vlog trigger for a single 
    /// <see cref="Core.Entities.SwabbrUser"/>.
    /// </summary>
    public sealed class VlogTriggerWrapper
    {

        public Guid UserId { get; set; }

        public DateTimeOffset UserTriggerMinute { get; set; }

        public int VlogRequestTimeoutMinutes { get; set; }

    }

}
