namespace Swabbr.Api.Configuration
{

    /// <summary>
    /// Configuration for Jwt authentication settings.
    /// TODO <see cref="uint"/>?
    /// </summary>
    public class JwtConfiguration
    {

        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public int ExpireDays { get; set; }
    }

}
