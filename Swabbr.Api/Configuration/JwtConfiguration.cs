namespace Swabbr.Api.Configuration
{

    /// <summary>
    /// Configuration for Jwt authentication settings.
    /// </summary>
    public class JwtConfiguration
    {

        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public uint ExpireDays { get; set; }

    }

}
