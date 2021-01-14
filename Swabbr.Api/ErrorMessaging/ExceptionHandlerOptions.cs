namespace Swabbr.Api.ErrorMessaging
{
    /// <summary>
    ///     Options for our exception handler.
    /// </summary>
    public class ExceptionHandlerOptions
    {
        /// <summary>
        ///     Path of the controller that handles our errors.
        /// </summary>
        public string ErrorControllerPath { get; set; }
    }
}
