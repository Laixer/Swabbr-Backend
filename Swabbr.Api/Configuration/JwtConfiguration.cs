namespace Swabbr.Api.Options
{
    /// <summary>
    /// Configuration for Jwt authentication settings
    /// </summary>
    public class JwtConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int ExpireDays { get; set; }
    }
}