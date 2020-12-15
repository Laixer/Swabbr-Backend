namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Represents an error message.
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        ///     Message to display.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        ///     Represents the error status code.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
