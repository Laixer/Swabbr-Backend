namespace Swabbr.Api.Options
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int ExpireDays { get; set; }
    }
}