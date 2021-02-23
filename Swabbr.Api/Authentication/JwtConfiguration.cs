namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Configuration for Jwt token settings.
    /// </summary>
    public class JwtConfiguration
    {
        /// <summary>
        ///     Token secret key.
        /// </summary>
        public string SignatureKey { get; set; }

        /// <summary>
        ///     Token issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        ///     Token audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        ///     Token expire time in minutes.
        /// </summary>
        public int TokenValidity { get; set; }

        /// <summary>
        ///     Refresh token expire time in minutes.
        /// </summary>
        public int RefreshTokenValidity { get; set; }
    }
}
