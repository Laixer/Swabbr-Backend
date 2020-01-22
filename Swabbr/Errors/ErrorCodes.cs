namespace Swabbr.Api.Errors
{
    public static class ErrorCodes
    {
        /// <summary>
        /// Indicates that a specified entity could not be created since it already existed during the execution of the request.
        /// </summary>
        public const int ENTITY_ALREADY_EXISTS = 601;

        /// <summary>
        /// Indicates that a specified entity could not be found.
        /// </summary>
        public const int ENTITY_NOT_FOUND = 602;

        /// <summary>
        /// Indicates that access to a specified resource is not allowed.
        /// </summary>
        public const int ACCESS_DENIED = 700;
    }
}
