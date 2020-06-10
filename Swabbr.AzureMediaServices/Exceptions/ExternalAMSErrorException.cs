using Swabbr.Core.Exceptions;
using System;

namespace Swabbr.AzureMediaServices.Exceptions
{
    /// <summary>
    /// Indicates an external error in Azure Media Services.
    /// </summary>
    public sealed class ExternalAMSErrorException : ExternalErrorException
    {
        public ExternalAMSErrorException(string message) : base(message)
        {
        }

        public ExternalAMSErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ExternalAMSErrorException()
        {
        }
    }
}
