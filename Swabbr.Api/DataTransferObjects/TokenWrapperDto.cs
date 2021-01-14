using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for when a user signed in.
    /// </summary>
    public record TokenWrapperDto
    {
        /// <summary>
        ///     The acces token.
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        ///     The date at which the access token was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; init; }

        /// <summary>
        ///     Timespan indicating how long the access token is valid.
        /// </summary>
        public TimeSpan TokenExpirationTimeSpan { get; init; }
    }
}
