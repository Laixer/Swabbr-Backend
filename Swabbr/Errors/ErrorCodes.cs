namespace Swabbr.Api.Errors
{
    public static class ErrorCodes
    {
        /// <summary>
        /// Indicates that a specified entity could not be created since it already existed during
        /// the execution of the request.
        /// </summary>
        public const int ENTITY_ALREADY_EXISTS = 601;

        /// <summary>
        /// Indicates that a specified entity could not be found.
        /// </summary>
        public const int ENTITY_NOT_FOUND = 602;

        /// <summary>
        /// Indicates that access to a specified resource is not allowed.
        /// </summary>
        public const int INSUFFICIENT_ACCESS_RIGHTS = 700;

        /// <summary>
        /// Indicates that a registration has failed to complete succesfully.
        /// </summary>
        public const int REGISTRATION_FAILED = 701;

        /// <summary>
        /// Indicates that a login could not be completed succesfully.
        /// </summary>
        public const int LOGIN_FAILED = 702;

        /// <summary>
        /// Indicates that an external process or service has encountered an error.
        /// </summary>
        public const int EXTERNAL_ERROR = 800;
    }
}