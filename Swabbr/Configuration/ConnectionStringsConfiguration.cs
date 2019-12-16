namespace Swabbr.Api.Options
{
    public enum ConnectionStringMode
    {
        Azure,
        Emulator
    }

    public class ConnectionStringsConfiguration
    {
        public ConnectionStringMode Mode { get; set; }
        public string Azure { get; set; }
        public string Emulator { get; set; }

        public string ActiveConnectionString =>
            Mode == ConnectionStringMode.Azure ? Azure : Emulator;
    }
}