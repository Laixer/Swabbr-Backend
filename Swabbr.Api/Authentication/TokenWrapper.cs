using System;

namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Wrapper around an access token and its metadata.
    /// </summary>
    internal sealed class TokenWrapper
    {
        /// <summary>
        ///     The id of the authenticating user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The acces token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     The refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     The date at which the access token was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Access token validity in minutes.
        /// </summary>
        public int TokenExpirationInMinutes { get; set; }

        /// <summary>
        ///     Refresh token validity in minutes.
        /// </summary>
        public int RefreshTokenExpirationInMinutes { get; set; }
    }
}
