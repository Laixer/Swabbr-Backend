using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for when a user signed in.
    /// </summary>
    public record TokenWrapperDto
    {
        /// <summary>
        ///     The id of the authenticating user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The acces token.
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        ///     The refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     The date at which the access token was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; init; }

        /// <summary>
        ///     Timespan indicating how long the access token is valid in minutes.
        /// </summary>
        public int TokenExpirationInMinutes { get; set; }

        /// <summary>
        ///     Timespan indicating how long the refresh token is valid in minutes.
        /// </summary>
        public int RefreshTokenExpirationInMinutes { get; set; }
    }
}
