namespace Swabbr.AzureFunctions.Parsing
{
    /// <summary>
    /// Wrapper for the event grid message of an AMS job result.
    /// </summary>
    public sealed class AMSEventGridJobResultMessage : AMSEventGridMessageBase
    {
        /// <summary>
        /// Contains all our outputs.
        /// </summary>
        public AMSJobResultMessage Data { get; set; }
    }
}
