using System;

namespace Swabbr.AzureFunctions.Parsing
{
    /// <summary>
    /// Base for an eventgrid message.
    /// </summary>
    public class AMSEventGridMessageBase
    {
        /// <summary>
        /// The resourse subject name.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The type of the event as string value.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// The time at which the event fired.
        /// </summary>
        public DateTimeOffset EventTime { get; set; }
    }
}
