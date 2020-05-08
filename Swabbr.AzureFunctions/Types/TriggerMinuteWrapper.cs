using System;

namespace Swabbr.AzureFunctions.Types
{

    /// <summary>
    /// JSON wrapper for sending a trigger minute.
    /// </summary>
    public sealed class TriggerMinuteWrapper
    {

        public DateTimeOffset TriggerMinute { get; set; }

    }

}
