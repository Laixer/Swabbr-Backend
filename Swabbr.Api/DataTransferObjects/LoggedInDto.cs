using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for when a user signed in.
    /// </summary>
    public class LoggedInDto
    {
        /// <summary>
        ///     The acces token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     The date at which the access token was created.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        ///     Timespan indicating how long the access token is valid.
        /// </summary>
        public TimeSpan TokenExpirationTimeSpan { get; set; }
    }
}
