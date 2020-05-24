namespace Swabbr.Api.Errors
{

    public static class ErrorCodes
    {
        /// <summary>
        /// Indicates that a specified entity could not be created since it already existed during
        /// the execution of the request.
        /// </summary>
        public const int EntityAlreadyExists = 601;

        /// <summary>
        /// Indicates that a specified entity could not be found.
        /// </summary>
        public const int EntityNotFound = 602;

        /// <summary>
        /// Indicates that the received input was invalid.
        /// </summary>
        public const int InvalidInput = 603;

        /// <summary>
        /// Indicates that the requested operation was invalid.
        /// </summary>
        public const int InvalidOperation = 604;

        /// <summary>
        /// Indicates that access to a specified resource is not allowed.
        /// </summary>
        public const int InsufficientAccessRights = 700;

        /// <summary>
        /// Indicates that a registration has failed to complete successfully.
        /// </summary>
        public const int RegistrationFailed = 701;

        /// <summary>
        /// Indicates that a login could not be completed successfully.
        /// </summary>
        public const int LoginFailed = 702;

        /// <summary>
        /// Indicates that an external process or service has encountered an error.
        /// </summary>
        public const int ExternalError = 800;
    }
}