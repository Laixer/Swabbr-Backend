namespace Swabbr.Api.Configuration
{

    /// <summary>
    /// Configuration for Jwt authentication settings.
    /// </summary>
    public class JwtConfiguration
    {

        /// <summary>
        /// Token secret key.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Token issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Token expire time in minutes.
        /// </summary>
        public uint ExpireMinutes { get; set; }

    }

}
