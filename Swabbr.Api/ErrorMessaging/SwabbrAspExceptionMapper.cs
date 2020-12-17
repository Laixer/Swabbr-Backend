using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Types;
using System.Net;

namespace Swabbr.Api.ErrorMessaging
{
    /// <summary>
    ///     Swabbr exception mapper for the ASP framework.
    /// </summary>
    public class SwabbrAspExceptionMapper : IExceptionMapper<SwabbrCoreException>
    {
        /// <summary>
        ///     Builds an <see cref="ErrorMessage"/>.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="statusCode">Status code to display.</param>
        /// <returns>Instance of<see cref="ErrorMessage"/>.</returns>
        protected static ErrorMessage BuildMessage(string message, HttpStatusCode statusCode)
            => new ErrorMessage
            {
                Message = message,
                StatusCode = (int)statusCode
            };

        /// <summary>
        ///     Converts an exception to an error message.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        /// <returns>Error message.</returns>
        public ErrorMessage Map(SwabbrCoreException exception)
            => exception switch
            {
                ConfigurationRangeException _ => BuildMessage("Application was unable to process the request", HttpStatusCode.InternalServerError),
                ConfigurationException _ => BuildMessage("Application was unable to process the request", HttpStatusCode.InternalServerError),
                EntityNotFoundException _ => BuildMessage("Requested entity was not found", HttpStatusCode.NotFound),
                FileNotFoundException _ => BuildMessage("Required file was not found", HttpStatusCode.NotFound),
                InvalidProfileImageStringException _ => BuildMessage("", HttpStatusCode.BadRequest),
                NicknameExistsException _ => BuildMessage("Nickname already exists", HttpStatusCode.BadRequest),
                NotAllowedException _ => BuildMessage("The operation is not allowed", HttpStatusCode.Conflict),
                OperationAbortedException _ => BuildMessage("The operation was cancelled", HttpStatusCode.BadRequest),
                ReferencedEntityNotFoundException _ => BuildMessage("Requested referenced entity was not found", HttpStatusCode.NotFound),
                StorageException _ => BuildMessage("Application storage was unable to process request", HttpStatusCode.InternalServerError),
                _ => BuildMessage("Application was unable to process the request", HttpStatusCode.InternalServerError)
            };
    }
}
