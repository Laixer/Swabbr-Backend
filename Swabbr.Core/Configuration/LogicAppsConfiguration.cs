namespace Swabbr.Core.Configuration
{

    /// <summary>
    /// Contains configuration values for our backend logic apps.
    /// TODO This shouldn't be in the core - it's implementation specific.
    /// </summary>
    public sealed class LogicAppsConfiguration
    {

        public string EndpointUserConnectTimeout { get; set; }

    }

}
